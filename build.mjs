import { build } from 'esbuild';
import fs from 'fs';
const r = await build({ entryPoints: ['src/view/main.ts'], bundle: true, minify: true, format: 'iife', target: 'es2020', write: false });
const js = r.outputFiles[0].text.replace(/<\/script/g, '<\\/script');
const tpl = fs.readFileSync('src/view/template.html', 'utf8');
fs.mkdirSync('dist', { recursive: true });
fs.writeFileSync('dist/fantastik-dunya.html', tpl.replace('/*APP*/', () => js));
console.log('ok', (fs.statSync('dist/fantastik-dunya.html').size / 1024).toFixed(1), 'KB');
