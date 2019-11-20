# TestSvgLoader
Example that shows how to load a SVG using Skia. 

It also Works in WebAssembly!

```
<local:SvgControl SourceFromEmbeddedResource="sonic.svg" />
```

Check this line. 

As today, we cannot use a Content file directly from WebAssembly. For now, we need to change the **Build action** to **Embedded Resource**.
Notice that the `SourceFromEmbeddedResource` is different from the `Source` property. 
