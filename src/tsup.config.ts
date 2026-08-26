import { defineConfig } from 'tsup'
export default defineConfig({
    entry: ['./main.ts'],
    format: ['iife'],
    splitting: false,
    sourcemap: true,
    clean: true,
    // outDir: './BlazorAdmin/wwwroot/js',
    // target: ['es5'],
    // minify: "terser",
    esbuildOptions(options, context) {
        options.outdir = undefined
        options.outfile = './BlazorAdmin/wwwroot/js/BlazorAdmin-project.js'
    },
})
