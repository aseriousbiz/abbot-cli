﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Serious.Abbot.CommandLine {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Serious.Abbot.CommandLine.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string Auth_Message {
            get {
                return ResourceManager.GetString("Auth_Message", resourceCulture);
            }
        }
        
        internal static string Initialized_Abbot_Directory {
            get {
                return ResourceManager.GetString("Initialized_Abbot_Directory", resourceCulture);
            }
        }
        
        internal static string Directory_Does_Not_Exist {
            get {
                return ResourceManager.GetString("Directory_Does_Not_Exist", resourceCulture);
            }
        }
        
        internal static string Directory_Is_Not_Abbot_Development_Enviroment {
            get {
                return ResourceManager.GetString("Directory_Is_Not_Abbot_Development_Enviroment", resourceCulture);
            }
        }
        
        internal static string Abbot_Directory_Not_Authenticated {
            get {
                return ResourceManager.GetString("Abbot_Directory_Not_Authenticated", resourceCulture);
            }
        }
    }
}