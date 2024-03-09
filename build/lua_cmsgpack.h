#ifndef LUA_CMSGPACK_H
#define LUA_CMSGPACK_H


LUA_API int luaopen_cmsgpack(lua_State *L);
LUA_API int luaopen_cmsgpack_safe(lua_State *L);
int arrint_to_msgpack(int *arr, int len, unsigned char** pack);

// Hx@2023-04-06: append时自定义扩容回调
typedef void *(*mp_append_realloc) (size_t oldsz, size_t newsz);
// Hx@2023-04-06: 打包到自定义内存
size_t mp_pack_buf(lua_State *L, mp_append_realloc realloc);

#endif
