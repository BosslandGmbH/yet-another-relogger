﻿using System;
using System.Collections.Generic;
using System.Linq;
using YetAnotherRelogger.Helpers.Hotkeys.Actions;

namespace YetAnotherRelogger.Helpers.Hotkeys
{
    public class ActionContainer
    {
        private static HashSet<IHotkeyAction> ActionList;

        static ActionContainer()
        {
            ActionList = new HashSet<IHotkeyAction>();
            // Create list
            ActionList.Add(new RepositionAll());
            ActionList.Add(new RepositionCurrent());
            ActionList.Add(new FullScreen());
            ActionList.Add(new ResizeCurrent());
        }

        public ActionContainer()
        {
            ActionList = new HashSet<IHotkeyAction>();
            // Create list
            ActionList.Add(new RepositionAll());
            ActionList.Add(new RepositionCurrent());
            ActionList.Add(new FullScreen());
            ActionList.Add(new ResizeCurrent());

            // Create Name and Version list
            Actions = new List<Action>();
            foreach (IHotkeyAction a in ActionList)
            {
                var action = new Action {Name = a.Name, Version = a.Version, Description = a.Description};
                Actions.Add(action);
            }
        }

        public List<Action> Actions { get; private set; }

        /// <summary>
        ///     Get Action by name and version
        /// </summary>
        /// <param name="name">Action Name</param>
        /// <param name="version">Action Version</param>
        /// <returns>returns object</returns>
        public static IHotkeyAction GetAction(string name, Version version)
        {
            IHotkeyAction ret = ActionList.FirstOrDefault(x => x.Name == name && x.Version == version);
            return ret;
        }
    }
}