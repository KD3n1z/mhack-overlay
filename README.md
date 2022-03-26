[![made-with-python](https://img.shields.io/badge/Made%20with-C%23-blueviolet)](https://docs.microsoft.com/en-us/dotnet/csharp/)
![downloads](https://img.shields.io/github/downloads/KD3n1z/mhack-overlay/total)
![license](https://img.shields.io/github/license/KD3n1z/mhack-overlay)

# m hack overlay
Simple and fully customizable overlay.
<br><br>
### How does formatting work?
It's very simple: program replaces %paramName to parameter value.
<br><br>
### Built-in parameters.
• <code>%freerammb</code> - free ram in mb<br>
• <code>%freeramkb</code> - free ram in kb<br>
• <code>%line</code> - new line<br>
• <code>%percent</code> - "%"<br>
• <code>%rmb</code> - is right mouse button down?<br>
• <code>%rmb</code> - is left mouse button down?<br>
• <code>%rcps</code> - right mouse button clicks per second<br>
• <code>%lcps</code> - left mouse button clicks per second<br>
• <code>%fixedlcps</code> - left mouse button clicks per second, and space at the end if value is smaller than 10<br>
• <code>%fixedrcps</code> - right mouse button clicks per second, and space at the end if value is smaller than 10<br>
• <code>%space</code> - " "<br>
• <code>%timer</code> - timer in format hh:mm:ss:mls (bind - <code>num lock</code>)<br>
<br><br>
### All about extensions.
• <code>Extensions</code> are console programs that extend the functionality of the overlay.<br>
• M Hack Overlay runs them in the background and reads their output.<br>
• First of all, the extension prints out its title. (Example: <code>title:best extension</code>).<br>
• Then, it starts constantly displaying the name of the parameters and their values. (Example: <code>weather:rainy</code>)<br>
• Extensions stored in %appdata%/Shu/Overlay/exts/
