const path = require('path');

module.exports = {
  entry: './TypeScript/parquetProcessor.ts',
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: ['.tsx', '.ts', '.js'],
  },
  output: {
    filename: 'parquetProcessor.js',
    path: path.resolve(__dirname, 'wwwroot/js'),
    library: {
      type: 'window',
      name: 'ParquetLogProcessor'
    },
    clean: true,
  },
  mode: 'development',
  devtool: 'source-map',
};