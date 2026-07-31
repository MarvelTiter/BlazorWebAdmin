import { defineConfig } from 'tsup'
export default defineConfig({
    entry: ['./Shared/BlazorTemplate.UI.Shared/TsHelper/shared.ts'],
    splitting: false,
    sourcemap: true,
    clean: true,
    // outDir: './BlazorAdmin/wwwroot/js',
    // target: ['es5'],
    // minify: "terser",
    esbuildOptions(options, context) {
        options.outdir = undefined
        options.outfile = './Shared/BlazorTemplate.UI.Shared/wwwroot/js/shared-project.js'
    },
})
