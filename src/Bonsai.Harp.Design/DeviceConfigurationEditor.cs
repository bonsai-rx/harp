﻿using Bonsai.Design;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Bonsai.Harp.Design
{
    /// <summary>
    /// Provides a user interface editor that displays a dialog box for inspecting
    /// and updating the current device firmware.
    /// </summary>
    public class DeviceConfigurationEditor : WorkflowComponentEditor
    {
        /// <inheritdoc/>
        public override bool EditComponent(ITypeDescriptorContext context, object component, IServiceProvider provider, IWin32Window owner)
        {
            if (provider != null)
            {
                var editorState = (IWorkflowEditorState)provider.GetService(typeof(IWorkflowEditorState));
                if (editorState != null)
                {
                    if (editorState.WorkflowRunning)
                    {
                        throw new InvalidOperationException(Properties.Resources.WorkflowRunning_Error);
                    }

                    var device = (Device)component;
                    using var editorForm = new DeviceConfigurationDialog(device);
                    try { editorForm.ShowDialog(owner); }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

            return false;
        }
    }
}
