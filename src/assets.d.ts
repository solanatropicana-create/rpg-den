// Blender asset'leri esbuild ile ikili olarak gömülür (build.mjs: loader '.glb': 'binary')
declare module '*.glb' {
  const data: Uint8Array;
  export default data;
}
