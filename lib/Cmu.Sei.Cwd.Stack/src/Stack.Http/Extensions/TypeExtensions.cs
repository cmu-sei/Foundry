/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Stack.Http
{
    public static class TypeExtensions
    {
        static IEnumerable<AssemblyName> GetMatchingAssemblyNames(string assemblyName)
        {
            var assemblyRefs = Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(a => a.Name.ToLower().Contains(assemblyName.ToLower())).ToList();

            var executingAssembly = Assembly.GetExecutingAssembly().GetName();
            if (executingAssembly.Name.ToLower().Contains(assemblyName.ToLower()) && !assemblyRefs.Contains(executingAssembly))
            {
                assemblyRefs.Add(executingAssembly);
            }

            var callingAssembly = Assembly.GetCallingAssembly().GetName();
            if (callingAssembly.Name.ToLower().Contains(assemblyName.ToLower()) && !assemblyRefs.Contains(callingAssembly))
            {
                assemblyRefs.Add(callingAssembly);
            }

            if (!assemblyRefs.Any())
            {
                assemblyRefs.Add(new AssemblyName(assemblyName));
            }

            return assemblyRefs;
        }

        /// <summary>
        /// perform an action on the matched type in an assembly if the assemblyName matches
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assemblyName"></param>
        /// <param name="process"></param>
        public static void ProcessTypeOf(this Type type, string assemblyName, Action<Type> process)
        {
            var assemblyRefs = GetMatchingAssemblyNames(assemblyName);

            var matches = new List<Type>();

            foreach (var assemblyRef in assemblyRefs)
            {
                var assembly = Assembly.Load(assemblyRef);

                if (type.GetTypeInfo().IsGenericType)
                {
                    matches.AddRange(assembly.GetTypes().Where(p => p != type && p.IsSubclassOfGeneric(type)));
                }
                else
                {
                    matches.AddRange(assembly.GetTypes().Where(p => p != type && p.GetTypeInfo().IsSubclassOf(type)));
                }
            }

            matches.ForEach(m => process(m));
        }

        public static bool IsSubclassOfGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.GetTypeInfo().BaseType;
            }
            return false;
        }

        public static bool IsGenericTypeOf(this Type t, Type genericDefinition)
        {
            return IsGenericTypeOf(t, genericDefinition, out Type[] parameters);
        }

        public static bool IsGenericTypeOf(this Type t, Type genericDefinition, out Type[] genericParameters)
        {
            genericParameters = new Type[] { };
            if (!genericDefinition.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            var isMatch = t.GetTypeInfo().IsGenericType &&
                t.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
            if (!isMatch && t.GetTypeInfo().BaseType != null)
            {
                isMatch = IsGenericTypeOf(t.GetTypeInfo().BaseType, genericDefinition, out genericParameters);
            }
            if (!isMatch && genericDefinition.GetTypeInfo().IsInterface && t.GetInterfaces().Any())
            {
                foreach (var i in t.GetInterfaces())
                {
                    if (i.IsGenericTypeOf(genericDefinition, out genericParameters))
                    {
                        isMatch = true;
                        break;
                    }
                }
            }

            if (isMatch && !genericParameters.Any())
            {
                genericParameters = t.GetGenericArguments();
            }
            return isMatch;
        }
    }
}

