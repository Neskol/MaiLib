# MaiLib

## A library for processing maimai charts
> One demo implementation is MaichartConverter to convert between Simai and Ma2. Please see [MaichartConverter](https://github.com/Neskol/MaichartConverter) for more information.

### Build
    git clone
    dotnet build
### Usage
- The basic parts of this is `Chart` - you could use this to construct different charts.
- This library follows Tokenizer - Parser - Compiler format to process files and uses Abstract Syntax Tree for grammer decomposing when implementing Simai and Ma2 format support. If you were to extend format supporting, make sure you also implement your Tokenizer, Parser, and Compiler, and make them follow Interface `ITokenizer`, `IParser`, and `ICompiler`.
- The `Chart` base class is a intermediate format to achieve inter-format compatability. If you are adding a new format, be sure to also create a class implementing this base class. The `Chart` class has already implemented the functions you need for a maimai chart, and you only need to implement several abstract method in this class to fit with your format.
- Then, you could create a maimai chart instance in your code. For example, creating a Ma2 chart could be done by `Chart ma2Chart = new Ma2();`

## Additional notice for Simai compatability
- As always, I think Simai is a language more focusing on charting rather than interpreting. I still have no idea why there is still no UI-based charting tool but let everyone learn this full-of-compromise language, especially after Festival added new features. This made interpreting Simai a PAIN since the idea how it converts between ticks and times are vague and unreasonable.
- For example, it defines a Slide note will have a wait time of 1 beat or one 1/4 note (or crotchet for music community) after its start tap. If your Slide starts longer or shorter than 1 quaver of current BPM, you will have to 1) change the BPM for this specific Slide or 2) define the time by [wait time##last time] - and calculating that is extremely time consuming when converting tick to time.
- I hope someone could develop a better language than Simai as a intermediate language between coding and charting. Thank you.

### Parameters notice
- music files should be named musicxxxxxx.mp3 which xxxxxx matches the music id specified in music.xml in each a000 folder, and compensate 0s at the front to 6 digits
- bga files should be named xxxxxx.mp4 which matches the music id specified in music.xml in each a000 folder, and compensate 0s at the front to 6 digits
- image folder should be structured in image/Texture2D/ and the files should start with UI_Jacket_xxxxxx.jpg which xxxxxx matches the music id specified in music.xml in each a000 folder, and compensate 0s at the front to 6 digits
- All of the rules specified above is in convenience for you to directly use after you obtain data from considerable ways
- The difficulty parameter is listed 0-4 as Basic to Re:Master. In MaiLib I specified rules for Easy and Utage but it takes times for me to figure it out, or you could implement on you own referring MaiLib code
- All of the path should end with path separator like "/" or "\". You cannot include quote signs in the path.
- If you have difficulty using the commands, please refer VSCode launch.json where I included several examples
- The whole program was planned to convert from ma2 to simai initially and all other features were developed after that, so there is a HUGE amount of compromises in code design which made it hard to read (but works so far). It would be most kind of you if you would like to help me fixing that

### Disclamer
- Copyrights of the works belong to each individual right holders. This tool is purely used as non-commercial and study purpose. You should find your way for any resource might be used and properly use at your own risk.
- If you would like to use the parser in your project, please refer [MaiLib](https://github.com/Neskol/MaiLib) and hopefully that helps!
