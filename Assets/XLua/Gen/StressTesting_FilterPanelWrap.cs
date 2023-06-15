#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class StressTestingFilterPanelWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(StressTesting.FilterPanel);
			Utils.BeginObjectRegister(type, L, translator, 0, 2, 4, 4);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsShowPushInterfaceInfo", _m_IsShowPushInterfaceInfo);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsShowRequestInterfaceInfo", _m_IsShowRequestInterfaceInfo);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "LuaScript", _g_get_LuaScript);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "helpButton", _g_get_helpButton);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "helpCloseButton", _g_get_helpCloseButton);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "filterInputField", _g_get_filterInputField);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "LuaScript", _s_set_LuaScript);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "helpButton", _s_set_helpButton);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "helpCloseButton", _s_set_helpCloseButton);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "filterInputField", _s_set_filterInputField);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new StressTesting.FilterPanel();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to StressTesting.FilterPanel constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsShowPushInterfaceInfo(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    PushInterfaceResponse.Types.MessageInterfaceInfo _info = (PushInterfaceResponse.Types.MessageInterfaceInfo)translator.GetObject(L, 2, typeof(PushInterfaceResponse.Types.MessageInterfaceInfo));
                    
                        var gen_ret = gen_to_be_invoked.IsShowPushInterfaceInfo( _info );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsShowRequestInterfaceInfo(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    RequestInterfaceResponse.Types.MessageInterfaceInfo _info = (RequestInterfaceResponse.Types.MessageInterfaceInfo)translator.GetObject(L, 2, typeof(RequestInterfaceResponse.Types.MessageInterfaceInfo));
                    
                        var gen_ret = gen_to_be_invoked.IsShowRequestInterfaceInfo( _info );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_LuaScript(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.LuaScript);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_helpButton(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.helpButton);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_helpCloseButton(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.helpCloseButton);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_filterInputField(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.filterInputField);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_LuaScript(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.LuaScript = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_helpButton(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.helpButton = (UnityEngine.UI.Button)translator.GetObject(L, 2, typeof(UnityEngine.UI.Button));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_helpCloseButton(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.helpCloseButton = (UnityEngine.UI.Button)translator.GetObject(L, 2, typeof(UnityEngine.UI.Button));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_filterInputField(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                StressTesting.FilterPanel gen_to_be_invoked = (StressTesting.FilterPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.filterInputField = (UnityEngine.UI.InputField)translator.GetObject(L, 2, typeof(UnityEngine.UI.InputField));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
