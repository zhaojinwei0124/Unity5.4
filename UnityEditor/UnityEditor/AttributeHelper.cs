using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace UnityEditor
{
	internal class AttributeHelper
	{
		private struct MonoGizmoMethod
		{
			public MethodInfo drawGizmo;

			public Type drawnType;

			public int options;
		}

		private struct MonoMenuItem
		{
			public string menuItem;

			public string execute;

			public string validate;

			public int priority;

			public int index;

			public Type type;
		}

		internal class CompareMenuIndex : IComparer
		{
			int IComparer.Compare(object xo, object yo)
			{
				AttributeHelper.MonoMenuItem monoMenuItem = (AttributeHelper.MonoMenuItem)xo;
				AttributeHelper.MonoMenuItem monoMenuItem2 = (AttributeHelper.MonoMenuItem)yo;
				if (monoMenuItem.priority != monoMenuItem2.priority)
				{
					return monoMenuItem.priority.CompareTo(monoMenuItem2.priority);
				}
				return monoMenuItem.index.CompareTo(monoMenuItem2.index);
			}
		}

		private struct MonoCreateAssetItem
		{
			public string menuItem;

			public string fileName;

			public int order;

			public Type type;
		}

		[CompilerGenerated]
		private sealed class <CallMethodsWithAttribute>c__Iterator3<T> : IDisposable, IEnumerator, IEnumerable, IEnumerable<T>, IEnumerator<T>
		{
			internal Assembly[] <$s_302>__0;

			internal int <$s_303>__1;

			internal Assembly <assembly>__2;

			internal Type[] <$s_304>__3;

			internal int <$s_305>__4;

			internal Type <type>__5;

			internal MethodInfo[] <$s_306>__6;

			internal int <$s_307>__7;

			internal MethodInfo <method>__8;

			internal Type attributeType;

			internal object[] arguments;

			internal int $PC;

			internal T $current;

			internal Type <$>attributeType;

			internal object[] <$>arguments;

			T IEnumerator<T>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				AttributeHelper.<CallMethodsWithAttribute>c__Iterator3<T> <CallMethodsWithAttribute>c__Iterator = new AttributeHelper.<CallMethodsWithAttribute>c__Iterator3<T>();
				<CallMethodsWithAttribute>c__Iterator.attributeType = this.<$>attributeType;
				<CallMethodsWithAttribute>c__Iterator.arguments = this.<$>arguments;
				return <CallMethodsWithAttribute>c__Iterator;
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.<$s_302>__0 = EditorAssemblies.loadedAssemblies;
					this.<$s_303>__1 = 0;
					goto IL_140;
				case 1u:
					IL_F0:
					this.<$s_307>__7++;
					break;
				default:
					return false;
				}
				IL_FE:
				if (this.<$s_307>__7 >= this.<$s_306>__6.Length)
				{
					this.<$s_305>__4++;
				}
				else
				{
					this.<method>__8 = this.<$s_306>__6[this.<$s_307>__7];
					if (this.<method>__8.GetCustomAttributes(this.attributeType, false).Length > 0)
					{
						this.$current = (T)((object)this.<method>__8.Invoke(null, this.arguments));
						this.$PC = 1;
						return true;
					}
					goto IL_F0;
				}
				IL_11F:
				if (this.<$s_305>__4 < this.<$s_304>__3.Length)
				{
					this.<type>__5 = this.<$s_304>__3[this.<$s_305>__4];
					this.<$s_306>__6 = this.<type>__5.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					this.<$s_307>__7 = 0;
					goto IL_FE;
				}
				this.<$s_303>__1++;
				IL_140:
				if (this.<$s_303>__1 < this.<$s_302>__0.Length)
				{
					this.<assembly>__2 = this.<$s_302>__0[this.<$s_303>__1];
					this.<$s_304>__3 = this.<assembly>__2.GetTypes();
					this.<$s_305>__4 = 0;
					goto IL_11F;
				}
				this.$PC = -1;
				return false;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private static AttributeHelper.MonoGizmoMethod[] ExtractGizmos(Assembly assembly)
		{
			List<AttributeHelper.MonoGizmoMethod> list = new List<AttributeHelper.MonoGizmoMethod>();
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < methods.GetLength(0); j++)
				{
					MethodInfo methodInfo = methods[j];
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(DrawGizmo), false);
					object[] array2 = customAttributes;
					for (int k = 0; k < array2.Length; k++)
					{
						DrawGizmo drawGizmo = (DrawGizmo)array2[k];
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (parameters.Length != 2)
						{
							UnityEngine.Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take parameters (ComponentType, GizmoType) so will be ignored.", methodInfo.DeclaringType.FullName, methodInfo.Name));
						}
						else
						{
							AttributeHelper.MonoGizmoMethod item = default(AttributeHelper.MonoGizmoMethod);
							if (drawGizmo.drawnType == null)
							{
								item.drawnType = parameters[0].ParameterType;
							}
							else
							{
								if (!parameters[0].ParameterType.IsAssignableFrom(drawGizmo.drawnType))
								{
									UnityEngine.Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but the component type it applies to could not be determined.", methodInfo.DeclaringType.FullName, methodInfo.Name));
									goto IL_18E;
								}
								item.drawnType = drawGizmo.drawnType;
							}
							if (parameters[1].ParameterType != typeof(GizmoType) && parameters[1].ParameterType != typeof(int))
							{
								UnityEngine.Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take a second parameter of type GizmoType so will be ignored.", methodInfo.DeclaringType.FullName, methodInfo.Name));
							}
							else
							{
								item.drawGizmo = methodInfo;
								item.options = (int)drawGizmo.drawOptions;
								list.Add(item);
							}
						}
						IL_18E:;
					}
				}
			}
			return list.ToArray();
		}

		private static AttributeHelper.MonoMenuItem[] ExtractMenuCommands(Assembly assembly)
		{
			bool @bool = EditorPrefs.GetBool("InternalMode", false);
			Dictionary<string, AttributeHelper.MonoMenuItem> dictionary = new Dictionary<string, AttributeHelper.MonoMenuItem>();
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < methods.GetLength(0); j++)
				{
					MethodInfo methodInfo = methods[j];
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(MenuItem), false);
					object[] array2 = customAttributes;
					int k = 0;
					while (k < array2.Length)
					{
						MenuItem menuItem = (MenuItem)array2[k];
						AttributeHelper.MonoMenuItem value = (!dictionary.ContainsKey(menuItem.menuItem)) ? default(AttributeHelper.MonoMenuItem) : dictionary[menuItem.menuItem];
						if (!menuItem.menuItem.StartsWith("internal:", StringComparison.Ordinal))
						{
							value.menuItem = menuItem.menuItem;
							goto IL_E9;
						}
						if (@bool)
						{
							value.menuItem = menuItem.menuItem.Substring(9);
							goto IL_E9;
						}
						IL_144:
						k++;
						continue;
						IL_E9:
						value.type = type;
						if (menuItem.validate)
						{
							value.validate = methodInfo.Name;
						}
						else
						{
							value.execute = methodInfo.Name;
							value.index = j;
							value.priority = menuItem.priority;
						}
						dictionary[menuItem.menuItem] = value;
						goto IL_144;
					}
				}
			}
			AttributeHelper.MonoMenuItem[] array3 = dictionary.Values.ToArray<AttributeHelper.MonoMenuItem>();
			Array.Sort(array3, new AttributeHelper.CompareMenuIndex());
			return array3;
		}

		private static AttributeHelper.MonoMenuItem[] ExtractContextMenu(Type klass)
		{
			Dictionary<string, AttributeHelper.MonoMenuItem> dictionary = new Dictionary<string, AttributeHelper.MonoMenuItem>();
			MethodInfo[] methods = klass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.GetLength(0); i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ContextMenu), false);
				object[] array = customAttributes;
				for (int j = 0; j < array.Length; j++)
				{
					ContextMenu contextMenu = (ContextMenu)array[j];
					AttributeHelper.MonoMenuItem value = (!dictionary.ContainsKey(contextMenu.menuItem)) ? default(AttributeHelper.MonoMenuItem) : dictionary[contextMenu.menuItem];
					value.menuItem = contextMenu.menuItem;
					value.type = klass;
					value.execute = methodInfo.Name;
					dictionary[contextMenu.menuItem] = value;
				}
			}
			return dictionary.Values.ToArray<AttributeHelper.MonoMenuItem>();
		}

		private static string GetComponentMenuName(Type klass)
		{
			object[] customAttributes = klass.GetCustomAttributes(typeof(AddComponentMenu), false);
			if (customAttributes.Length > 0)
			{
				AddComponentMenu addComponentMenu = (AddComponentMenu)customAttributes[0];
				return addComponentMenu.componentMenu;
			}
			return null;
		}

		private static int GetComponentMenuOrdering(Type klass)
		{
			object[] customAttributes = klass.GetCustomAttributes(typeof(AddComponentMenu), false);
			if (customAttributes.Length > 0)
			{
				AddComponentMenu addComponentMenu = (AddComponentMenu)customAttributes[0];
				return addComponentMenu.componentOrder;
			}
			return 0;
		}

		private static AttributeHelper.MonoCreateAssetItem[] ExtractCreateAssetMenuItems(Assembly assembly)
		{
			List<AttributeHelper.MonoCreateAssetItem> list = new List<AttributeHelper.MonoCreateAssetItem>();
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				CreateAssetMenuAttribute createAssetMenuAttribute = (CreateAssetMenuAttribute)Attribute.GetCustomAttribute(type, typeof(CreateAssetMenuAttribute));
				if (createAssetMenuAttribute != null)
				{
					if (!type.IsSubclassOf(typeof(ScriptableObject)))
					{
						UnityEngine.Debug.LogWarningFormat("CreateAssetMenu attribute on {0} will be ignored as {0} is not derived from ScriptableObject.", new object[]
						{
							type.FullName
						});
					}
					else
					{
						string menuItem = (!string.IsNullOrEmpty(createAssetMenuAttribute.menuName)) ? createAssetMenuAttribute.menuName : ObjectNames.NicifyVariableName(type.Name);
						string text = (!string.IsNullOrEmpty(createAssetMenuAttribute.fileName)) ? createAssetMenuAttribute.fileName : ("New " + ObjectNames.NicifyVariableName(type.Name) + ".asset");
						if (!Path.HasExtension(text))
						{
							text += ".asset";
						}
						list.Add(new AttributeHelper.MonoCreateAssetItem
						{
							menuItem = menuItem,
							fileName = text,
							order = createAssetMenuAttribute.order,
							type = type
						});
					}
				}
			}
			return list.ToArray();
		}

		internal static ArrayList FindEditorClassesWithAttribute(Type attrib)
		{
			ArrayList arrayList = new ArrayList();
			foreach (Type current in EditorAssemblies.loadedTypes)
			{
				if (current.GetCustomAttributes(attrib, false).Length != 0)
				{
					arrayList.Add(current);
				}
			}
			return arrayList;
		}

		internal static object InvokeMemberIfAvailable(object target, string methodName, object[] args)
		{
			MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				return method.Invoke(target, args);
			}
			return null;
		}

		internal static bool GameObjectContainsAttribute(GameObject go, Type attributeType)
		{
			Component[] components = go.GetComponents(typeof(Component));
			for (int i = 0; i < components.Length; i++)
			{
				Component component = components[i];
				if (!(component == null))
				{
					Type type = component.GetType();
					if (type.GetCustomAttributes(attributeType, true).Length > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		[DebuggerHidden]
		internal static IEnumerable<T> CallMethodsWithAttribute<T>(Type attributeType, params object[] arguments)
		{
			AttributeHelper.<CallMethodsWithAttribute>c__Iterator3<T> <CallMethodsWithAttribute>c__Iterator = new AttributeHelper.<CallMethodsWithAttribute>c__Iterator3<T>();
			<CallMethodsWithAttribute>c__Iterator.attributeType = attributeType;
			<CallMethodsWithAttribute>c__Iterator.arguments = arguments;
			<CallMethodsWithAttribute>c__Iterator.<$>attributeType = attributeType;
			<CallMethodsWithAttribute>c__Iterator.<$>arguments = arguments;
			AttributeHelper.<CallMethodsWithAttribute>c__Iterator3<T> expr_23 = <CallMethodsWithAttribute>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		internal static string GetHelpURLFromAttribute(Type objectType)
		{
			HelpURLAttribute helpURLAttribute = (HelpURLAttribute)Attribute.GetCustomAttribute(objectType, typeof(HelpURLAttribute));
			return (helpURLAttribute == null) ? null : helpURLAttribute.URL;
		}
	}
}