﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Octgn.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Octgn.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static System.Drawing.Bitmap audio {
            get {
                object obj = ResourceManager.GetObject("audio", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        public static System.Drawing.Bitmap audiomute {
            get {
                object obj = ResourceManager.GetObject("audiomute", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to import collections
        ///
        ///class idict(collections.Mapping):
        ///
        ///    def __init__(self, pairs): 
        ///        &quot;&quot;&quot;Constructor: takes an iterable of tuples&quot;&quot;&quot; 
        ///        self._dict = {} 
        ///        for key, value in pairs: 
        ///            k = key.lower() 
        ///            self._dict[k] = (key, value) 
        ///
        ///    def __getitem__(self, key):
        ///        k = key.lower()
        ///        pair = self._dict[k]
        ///        return pair[1]
        ///
        ///    def __len__(self):
        ///        return len(self._dict)
        ///
        ///    def __iter__(self):
        ///        return (key for key, v [rest of string was truncated]&quot;;.
        /// </summary>
        public static string CaseInsensitiveDict {
            get {
                return ResourceManager.GetString("CaseInsensitiveDict", resourceCulture);
            }
        }
        
        public static System.IO.UnmanagedMemoryStream click {
            get {
                return ResourceManager.GetStream("click", resourceCulture);
            }
        }
        
        public static System.IO.UnmanagedMemoryStream logoff {
            get {
                return ResourceManager.GetStream("logoff", resourceCulture);
            }
        }
        
        public static System.IO.UnmanagedMemoryStream logon {
            get {
                return ResourceManager.GetStream("logon", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #Rotation constants
        ///Rot0 = 0
        ///Rot90 = 1
        ///Rot180 = 2
        ///Rot270 = 3
        ///
        ///def mute():
        ///  class Muted(object):
        ///    def __enter__(self):
        ///      return self
        ///    def __exit__(self, type, value, tb):
        ///      _api.Mute(False)
        ///  _api.Mute(True)
        ///  return Muted()
        ///
        ///def notify(message):
        ///  _api.Notify(message)
        ///
        ///def rnd(min, max):
        ///  return _api.Random(min, max)
        ///
        ///def confirm(message):
        ///  return _api.Confirm(message)
        ///
        ///def askInteger(question, defaultAnswer):
        ///  return _api.AskInteger(question, defaultAnswer)
        ///
        ///def [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PythonAPI {
            get {
                return ResourceManager.GetString("PythonAPI", resourceCulture);
            }
        }
        
        public static System.IO.UnmanagedMemoryStream whisper {
            get {
                return ResourceManager.GetStream("whisper", resourceCulture);
            }
        }
    }
}
