import { defineConfig } from 'tsup'
export default defineConfig({
    entry: ['./Shared/BlazorTemplates/BlazorTemplate.ClientCore/TsHelper/shared.ts'],
    format: ['iife'],
    splitting: false,
    sourcemap: true,
    clean: true,
    // outDir: './BlazorAdmin/wwwroot/js',
    // target: ['es5'],
    // minify: "terser",
    esbuildOptions(options, context) {
        options.outdir = undefined
        options.outfile = './Shared/BlazorTemplates/BlazorTemplate.ClientCore/wwwroot/js/shared-project.js'
    },
})
