﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CoolNameGenerator.Properties;

namespace CoolNameGenerator.Forms
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();

            Load += (s, e) => LocalizingControl(this);
        }

        
        protected virtual void LocalizingControl(Control ctrl)
        {
            if (string.IsNullOrEmpty(ctrl?.Text)) return;

            ctrl.Text = Localization.ResourceManager.GetString(ctrl.Text);

            var controls = GetControls(ctrl);

            foreach (Control c in controls.SkipWhile(x => x.GetType() == typeof(NumericUpDown)))
            {
                if (c.GetType() == typeof(MenuStrip))
                {
                    LocalizingMenuStrip((MenuStrip)c);
                    continue;
                }
                var replacmentText = Localization.ResourceManager.GetString(c.Text);
                if (!string.IsNullOrEmpty(replacmentText))
                {
                    c.Text = replacmentText;
                }
            }
        }
        protected virtual void LocalizingMenuStrip(MenuStrip menu)
        {
            var menuItems = GetToolStripMenuItems(menu);
            var items = GetToolStripItems(menu);

            foreach (ToolStripMenuItem item in menuItems)
            {
                var replacmentText = Localization.ResourceManager.GetString(item.Text);
                if (!string.IsNullOrEmpty(replacmentText))
                {
                    item.Text = replacmentText;
                }
            }

            foreach (ToolStripItem item in items)
            {
                var replacmentText = Localization.ResourceManager.GetString(item.Text);
                if (!string.IsNullOrEmpty(replacmentText))
                {
                    item.Text = replacmentText;
                }
            }
        }

        public IEnumerable<Control> GetControls(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>().ToArray();

            return controls.SelectMany(ctrl => GetControls(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        public IEnumerable<Control> GetControls(Control control)
        {
            var controls = control.Controls.Cast<Control>().ToArray();

            return controls.SelectMany(GetControls).Concat(controls);
        }

        public IEnumerable<ToolStripItem> GetToolStripItems(MenuStrip menu)
        {
            var items = menu.Items.Cast<ToolStripMenuItem>().ToArray();

            return items.SelectMany(GetToolStripItems).Concat(items);
        }
        public IEnumerable<ToolStripItem> GetToolStripItems(ToolStripMenuItem mewnItem)
        {
            var items = new List<ToolStripItem>();

            foreach (var item in mewnItem.DropDownItems)
            {
                if (item.GetType() == typeof(ToolStripMenuItem))
                {
                    items.AddRange(GetToolStripItems(item as ToolStripMenuItem));
                }
                else // ToolStripItem
                {
                    items.Add(item as ToolStripItem);
                }
            }

            return items;
        }

        public IEnumerable<ToolStripMenuItem> GetToolStripMenuItems(MenuStrip menu)
        {
            var items = menu.Items.Cast<ToolStripMenuItem>().ToArray();

            return items.SelectMany(GetToolStripMenuItems).Concat(items);
        }
        public IEnumerable<ToolStripMenuItem> GetToolStripMenuItems(ToolStripMenuItem mewnItem)
        {
            var items = new List<ToolStripMenuItem>();

            foreach (var item in mewnItem.DropDownItems)
            {
                if (item.GetType() == typeof(ToolStripMenuItem))
                {
                    items.Add(item as ToolStripMenuItem);
                    items.AddRange(GetToolStripMenuItems(item as ToolStripMenuItem));
                }
            }

            return items;
        }

    }
}