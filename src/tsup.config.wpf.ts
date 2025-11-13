import { defineConfig } from 'tsup'
export default defineConfig({
    entry: ['./main.ts'],
    splitting: false,
    sourcemap: true,
    clean: true,
    esbuildOptions(options, context) {
        options.outdir = undefined
        options.outfile = './BlazorAdmin.Wpf/wwwroot/js/blazor-admin-project.js'
    },
})
