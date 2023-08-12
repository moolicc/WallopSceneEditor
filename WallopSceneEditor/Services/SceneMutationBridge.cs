using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;
using Wallop.Shared.Messaging;
using Wallop.Shared.Messaging.Messages;
using static Avalonia.Media.Transformation.TransformOperation.DataLayout;

namespace WallopSceneEditor.Services
{
    public static class SceneMutationBridge
    {
        private readonly record struct ActorTableKey(string Layout, string Actor)
        {
            public override int GetHashCode()
                => Layout.GetHashCode() * Actor.GetHashCode() * 31;
        }

        public static StoredScene? EngineTruth;

        public static IEngineService? Engine;
        public static ISceneMutator? Mutator;

        public static bool Enabled { get; private set; }


        // Tables to lookup component names.
        // The keys should be their current, up-to-date names.
        // The values should be their names as of the instance they were equal in the EngineTruth.
        private static Dictionary<string, string> _directorNameTable;
        private static Dictionary<string, string> _layoutNameTable;
        private static Dictionary<ActorTableKey, string> _actorNameTable;

        static SceneMutationBridge()
        {
            _directorNameTable = new Dictionary<string, string>();
            _layoutNameTable = new Dictionary<string, string>();
            _actorNameTable = new Dictionary<ActorTableKey, string>();
        }


        public static void Enable()
        {
            if(Engine == null || Mutator == null)
            {
                throw new NullReferenceException("Engine and Mutator must both be set to enable the SceneMutationBridge.");
            }

            Mutator.OnDirectorRenamed += Mutator_OnDirectorRenamed;
            Mutator.OnLayoutRenamed += Mutator_OnLayoutRenamed;
            Mutator.OnActorRenamed += Mutator_OnActorRenamed;
            Mutator.OnValidatedLayout += Mutator_OnValidatedLayout;
            Mutator.OnValidateDirector += Mutator_OnValidateDirector;
            Mutator.OnValidateActor += Mutator_OnValidateActor;


            _layoutNameTable.Clear();
            _actorNameTable.Clear();

            if (EngineTruth != null)
            {
                foreach (var dir in EngineTruth.DirectorModules)
                {
                    _directorNameTable.Add(dir.InstanceName, dir.InstanceName);
                }
                foreach (var layout in EngineTruth.Layouts)
                {
                    _layoutNameTable.Add(layout.Name, layout.Name);
                    foreach (var actor in layout.ActorModules)
                    {
                        _actorNameTable.Add(new ActorTableKey(layout.Name, actor.InstanceName), actor.InstanceName);
                    }
                }
            }


            Enabled = true;
        }

        public static void Mutator_OnLayoutAdded(object? sender, Models.EventData.Mutator.LayoutEventArgs e)
        {
            if (e.HasError || Engine == null || EngineTruth == null || Mutator == null)
            {
                return;
            }

            _layoutNameTable.Add(e.LayoutName, e.LayoutName);

            var layout = Mutator.FindLayout(e.LayoutName)!;

            Engine.SendMessageAsync(new AddLayoutMessage(e.LayoutName, null, null, layout.Active, layout.ScreenIndex, new System.Numerics.Vector2(layout.RenderWidth, layout.RenderHeight), null));
            EngineTruth.Layouts.Add(new StoredLayout()
            {
                Name = e.LayoutName,
                ScreenIndex = layout.ScreenIndex,
                Active = layout.Active,
                RenderHeight = layout.RenderHeight,
                RenderWidth = layout.RenderWidth
            });
        }

        public static void Disable()
        {
            if ((Engine == null || Mutator == null) && Enabled)
            {
                throw new NullReferenceException("Engine and Mutator must both be set to disable the SceneMutationBridge.");
            }
            else if (!Enabled)
            {
                return;
            }

            Enabled = false;
        }


        private static void Mutator_OnDirectorRenamed(object? sender, Models.EventData.Mutator.MutatorValueChangedEventArgs<string> e)
        {
            if (e.HasError)
            {
                return;
            }
            var thruthName = _directorNameTable[e.OldValue];

            _directorNameTable.Remove(e.OldValue);
            _directorNameTable.Add(e.NewValue, thruthName);
        }

        private static void Mutator_OnLayoutRenamed(object? sender, Models.EventData.Mutator.MutatorValueChangedEventArgs<string> e)
        {
            if(e.HasError)
            {
                return;
            }


        }

        private static void Mutator_OnActorRenamed(object? sender, Models.EventData.Mutator.ActorValueChangedEventArgs<string> e)
        {
            if (e.HasError)
            {
                return;
            }
            KeyValuePair<ActorTableKey, string>? actorToMutate = null;
            foreach (var actorItem in _actorNameTable)
            {
                if(actorItem.Key.Layout == e.ParentLayout && actorItem.Key.Actor == e.NewValue)
                {
                    actorToMutate = actorItem;
                    break;
                }
            }

            // This will cause a problem is an actor was previously added, then renamed before being validated.
            // The truthiness of the actor will be that it simply does not exist, it won't exist in the name table,
            // and this exception will be raised.
            if(actorToMutate == null)
            {
                throw new KeyNotFoundException();
            }

            _actorNameTable.Remove(actorToMutate.Value.Key);
            _actorNameTable.Add(new ActorTableKey(actorToMutate.Value.Key.Layout, e.NewValue), actorToMutate.Value.Value);
        }

        private static void Mutator_OnValidatedLayout(object? sender, Models.EventData.Mutator.LayoutEventArgs e)
        {
            if (Engine == null || Mutator == null || e.HasError || EngineTruth == null)
            {
                return;
            } 

            var isNewLayout = !_layoutNameTable.TryGetValue(e.LayoutName, out var truthName);


            StoredLayout? truthLayout = null;
            StoredLayout existingLayout = Mutator.FindLayout(e.LayoutName)!;

            if (isNewLayout)
            {
                truthLayout = new StoredLayout();
                truthLayout.Name = e.LayoutName;
                EngineTruth.Layouts.Add(truthLayout);
                _layoutNameTable.Add(e.LayoutName, e.LayoutName);
            }
            else
            {
                truthLayout = EngineTruth.Layouts.First(l => l.Name == truthName);
            }

            // Find new data.
            string? newName = null;
            bool? newActive = null;
            int? newScreen = null;
            Vector2? newRenderSize = null;
            Vector4? newPresentationBounds = null;

            if(truthName != e.LayoutName)
            {
                newName = e.LayoutName;
            }

            if(existingLayout.Active != truthLayout.Active)
            {
                newActive = existingLayout.Active;
            }

            if (existingLayout.ScreenIndex != truthLayout.ScreenIndex)
            {
                newScreen = existingLayout.ScreenIndex;
            }

            if(existingLayout.RenderWidth != truthLayout.RenderWidth || existingLayout.RenderHeight != truthLayout.RenderHeight)
            {
                newRenderSize = new Vector2(existingLayout.RenderWidth, existingLayout.RenderHeight);
            }

            // TODO: Presentation bounds support.


            // Send data to engine.
            if(isNewLayout)
            {
                var dimensions = new Vector2(existingLayout.RenderWidth, existingLayout.RenderHeight);

                // TODO: Presentation bounds support.
                Engine.SendMessageAsync(new AddLayoutMessage(existingLayout.Name, null, null, existingLayout.Active, existingLayout.ScreenIndex, dimensions, null));
            }
            else
            {
                // TODO: Presentation bounds support.
                Engine.SendMessageAsync(new LayoutChangeMessage(existingLayout.Name, newName, newActive, newScreen, newRenderSize, newPresentationBounds));
            }


            // Updates to truth layout.

            // Rename the layout if necessary.
            if (truthName != e.LayoutName && truthName != null && newName != null)
            {
                var oldName = truthName;

                _layoutNameTable.Remove(oldName);
                _layoutNameTable.Add(newName, newName);


                // Update actors that depend on this layout from the actor table.
                KeyValuePair<ActorTableKey, string>? actorToMutate = null;
                foreach (var item in _actorNameTable)
                {
                    if (item.Key.Layout == oldName)
                    {
                        actorToMutate = item;
                        break;
                    }
                }

                if (actorToMutate != null)
                {
                    _actorNameTable.Remove(actorToMutate.Value.Key);
                    _actorNameTable.Add(new ActorTableKey(newName, actorToMutate.Value.Key.Actor), actorToMutate.Value.Value);
                }
            }

            if(truthLayout != null && newActive != null)
            {
                truthLayout.Active = newActive.Value;
            }
            if(truthLayout != null && newScreen != null)
            {
                truthLayout.ScreenIndex = newScreen.Value;
            }
            if(truthLayout != null && newRenderSize != null)
            {
                truthLayout.RenderWidth = (int)newRenderSize.Value.X;
                truthLayout.RenderHeight = (int)newRenderSize.Value.Y;
            }
        }

        private static void Mutator_OnValidateDirector(object? sender, Models.EventData.Mutator.DirectorEventArgs e)
        {
            if(Engine == null || Mutator == null || e.HasError || EngineTruth == null)
            {
                return;
            }

            // Send message to Engine.
            var isNewDirector = !_directorNameTable.TryGetValue(e.DirectorName, out var truthName);

            if(isNewDirector)
            {
                truthName = string.Empty;
            }

            StoredModule? truthDirector = null;
            StoredModule existingDirector = Mutator.FindDirector(e.DirectorName)!;

            if(isNewDirector)
            {
                truthDirector = new StoredModule();
                EngineTruth.DirectorModules.Add(truthDirector);
                _directorNameTable.Add(e.DirectorName, e.DirectorName);
            }
            else
            {
                truthDirector = EngineTruth.DirectorModules.FirstOrDefault(d => d.InstanceName == truthName);
            }

            string? newName = null;
            IEnumerable<KeyValuePair<string, string?>>? newSettings = null;

            if (truthName != e.DirectorName)
            {
                newName = e.DirectorName;
            }

            if(isNewDirector)
            {
                newSettings = new List<KeyValuePair<string, string?>>(existingDirector.Settings.Select(s => new KeyValuePair<string, string?>(s.Name, s.Value)));
            }
            else if(truthDirector != null)
            {
                var tempSettings = new List<KeyValuePair<string, string?>>();

                // Check added and/or changed settings.
                foreach (var existingSetting in existingDirector.Settings)
                {
                    var found = false;
                    foreach (var truthSetting in truthDirector.Settings)
                    {
                        if (existingSetting.Name == truthSetting.Name)
                        {
                            if (existingSetting.Value != truthSetting.Value)
                            {
                                tempSettings.Add(new KeyValuePair<string, string?>(existingSetting.Name, existingSetting.Value));
                            }
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        tempSettings.Add(new KeyValuePair<string, string?>(existingSetting.Name, existingSetting.Value));
                    }
                }

                // Check removed settings.
                foreach (var truthSetting in truthDirector.Settings)
                {
                    var found = false;
                    foreach (var existingSetting in existingDirector.Settings)
                    {
                        if (existingSetting.Name == truthSetting.Name)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        continue;
                    }

                    // TODO: In the future, handle removal of settings more properly.
                    tempSettings.Add(new KeyValuePair<string, string?>(truthSetting.Name, null));
                }

                newSettings = tempSettings;
            }


            // Send message to Engine.
            if (isNewDirector)
            {
                // TODO: Scene should be nullable here, as should the Value for settings.
                Engine.SendMessageAsync(new AddDirectorMessage(existingDirector.InstanceName, existingDirector.ModuleId, null, newSettings));
            }
            else
            {
                // TODO: Create and send DirectorChangeMessage.
                //Engine.SendMessageAsync(new ActorChangeMessage(existingActor.InstanceName, _layoutNameTable[e.ParentLayout], newName, newLayout, newSettings));
            }


            // If we updated the name of the actor, then also update our name table and truth to reflect that.
            if (truthName != e.DirectorName)
            {
                if(truthDirector != null)
                {
                    truthDirector.InstanceName = e.DirectorName;
                }
                _directorNameTable[e.DirectorName] = e.DirectorName;
            }
        }

        private static void Mutator_OnValidateActor(object? sender, Models.EventData.Mutator.ActorEventArgs e)
        {
            if (Engine == null || Mutator == null || e.HasError || EngineTruth == null)
            {
                return;
            }

            // Find the name of the actor as it exists in the engine.
            var actorKey = new ActorTableKey(e.ParentLayout, e.ActorName);
            var isNewActor = !_actorNameTable.TryGetValue(actorKey, out var truthName);

            if(isNewActor)
            {
                truthName = string.Empty;
            }

            // Find the actors as they exist both in truth and locally.
            StoredModule? truthActor = null;
            StoredModule existingActor = Mutator.FindActor(e.ParentLayout, e.ActorName)!;
            var truthLayout = EngineTruth.Layouts.FirstOrDefault(l => l.Name == e.ParentLayout)!;
            if (isNewActor)
            {
                truthActor = new StoredModule();
                truthLayout.ActorModules.Add(truthActor);
                _actorNameTable.Add(new ActorTableKey(e.ParentLayout, e.ActorName), e.ActorName);
            }
            else
            {
                var oldLayout = _layoutNameTable[e.ParentLayout];
                truthActor = truthLayout.ActorModules.FirstOrDefault(a => a.InstanceName == truthName);
            }

            string? newName = null;
            string? newLayout = null;
            IEnumerable<KeyValuePair<string, string?>>? newSettings = null;

            // Setup our actor's name.
            if (truthName != e.ActorName)
            {
                newName = e.ActorName;
            }

            // Setup our settings.
            if(isNewActor)
            {
                newSettings = new List<KeyValuePair<string, string?>>(existingActor.Settings.Select(s => new KeyValuePair<string, string?>(s.Name, s.Value)));
            }
            else if(truthActor != null)
            {
                var tempSettings = new List<KeyValuePair<string, string?>>();

                // Check added and/or changed settings.
                foreach (var existingSetting in existingActor.Settings)
                {
                    var found = false;
                    foreach (var truthSetting in truthActor.Settings)
                    {
                        if(existingSetting.Name == truthSetting.Name)
                        {
                            if(existingSetting.Value != truthSetting.Value)
                            {
                                tempSettings.Add(new KeyValuePair<string, string?>(existingSetting.Name, existingSetting.Value));
                            }
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        tempSettings.Add(new KeyValuePair<string, string?>(existingSetting.Name, existingSetting.Value));
                    }
                }

                // Check removed settings.
                foreach (var truthSetting in truthActor.Settings)
                {
                    var found = false;
                    foreach (var existingSetting in existingActor.Settings)
                    {
                        if (existingSetting.Name == truthSetting.Name)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        continue;
                    }

                    // TODO: In the future, handle removal of settings more properly.
                    tempSettings.Add(new KeyValuePair<string, string?>(truthSetting.Name, null));
                }

                newSettings = tempSettings;
            }
            
            // Send message to Engine.
            if(isNewActor)
            {
                Engine.SendMessageAsync(new AddActorMessage(existingActor.InstanceName, null, truthLayout.Name, existingActor.ModuleId, newSettings));
            }
            else
            {
                Engine.SendMessageAsync(new ActorChangeMessage(existingActor.InstanceName, truthLayout.Name, null, newName, newLayout, newSettings));
            }


            // If we updated the name of the actor, then also update our name table and truth to reflect that.
            if (truthName != e.ActorName)
            {
                if(truthActor != null)
                {
                    truthActor.InstanceName = e.ActorName;
                }
                _actorNameTable[actorKey] = e.ActorName;
            }
        }
    }
}
