import { defineConfig } from 'tsup'
// const fs = require('fs');
// const path = require('path');
// /**
//  * 遍历指定目录下的所有文件
//  * @param {*} dir 
//  */
// const getAllFile = function (dir) {
//     let res: any[] = []
//     function traverse(dir) {
//         fs.readdirSync(dir).forEach((file) => {
//             const pathname = path.join(dir, file)
//             if (fs.statSync(pathname).isDirectory()) {
//                 traverse(pathname)
//             } else {
//                 if (pathname.endWith('.razor.ts'))
//                     res.push({
//                         file,
//                         pathname
//                     })
//             }
//         })
//     }
//     traverse(dir)
//     return res;
// }
// var files = getAllFile('./')
// console.log(files)
export default defineConfig({
    entry: ['./main.ts'],
    splitting: false,
    sourcemap: true,
    clean: true,
    // outDir: './BlazorAdmin/wwwroot/js',
    // target: ['es5'],
    // minify: "terser",
    esbuildOptions(options, context) {
        options.outdir = undefined
        options.outfile = './BlazorAdmin/wwwroot/js/blazor-admin-project.js'
    },
})
