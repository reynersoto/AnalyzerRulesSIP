﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Analyzer1 {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CodeFixResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CodeFixResources() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Analyzer1.CodeFixResources", typeof(CodeFixResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Dividir lineas.
        /// </summary>
        internal static string GP001Fix {
            get {
                return ResourceManager.GetString("GP001Fix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Convertir en mayúscula..
        /// </summary>
        internal static string GP003TituloFix {
            get {
                return ResourceManager.GetString("GP003TituloFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Omitir error aplicando regla de todos modos..
        /// </summary>
        internal static string GP003TituloSkip {
            get {
                return ResourceManager.GetString("GP003TituloSkip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Agregar línea en blanco.
        /// </summary>
        internal static string GP004Fix {
            get {
                return ResourceManager.GetString("GP004Fix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Mover comentario a una nueva línea..
        /// </summary>
        internal static string GP005Fix {
            get {
                return ResourceManager.GetString("GP005Fix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Agregar punto al final del comentario.
        /// </summary>
        internal static string GP006Fix {
            get {
                return ResourceManager.GetString("GP006Fix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Asignar prefijo s_ a la variable static..
        /// </summary>
        internal static string GP007Fix {
            get {
                return ResourceManager.GetString("GP007Fix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Asignar prefijo t_ a la variable thread static..
        /// </summary>
        internal static string GP008Fix {
            get {
                return ResourceManager.GetString("GP008Fix", resourceCulture);
            }
        }
    }
}
