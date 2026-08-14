import { defineConfig } from 'tsup'
export default defineConfig({
    entry: ['./Shared/BlazorTemplate.ClientCore/TsHelper/shared.ts'],
    splitting: false,
    sourcemap: true,
    clean: true,
    // outDir: './BlazorAdmin/wwwroot/js',
    // target: ['es5'],
    // minify: "terser",
    esbuildOptions(options, context) {
        options.outdir = undefined
        options.outfile = './Shared/BlazorTemplate.ClientCore/wwwroot/js/shared-project.js'
    },
})
