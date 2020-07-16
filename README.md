
# ScLint [![GitHub license](https://img.shields.io/badge/license-apache2-blue.svg)](https://github.com/dawiddworak88/ASP.NET-Core-React-and-SSR/blob/master/LICENSE.md) ![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)

This repository contains a set of C# Roslyn code analyzers helping to maintain compliance with good practices while working on Sitecore projects.

## Installation guide

### Code analyzers
Run <em><b>root</b>\src\Roslyn\ScLint\ScLint\ScLint.Vsix\bin\Debug\ScLint.vsix</em> VSIX file to start installation process of extension on your local environment. Install the set of code analyzers to specific Visual Studio release by selecting target version in dialog box. After successful installation you can see the extension in <em>Tools - Extension and Updates...</em> (Visual Studio 2017) or <em>Extensions - Manage Extensions</em> (Visual Studio 2019) window.

## Content

### List of included C# code analyzers
Repository's directory "src" contains "Roslyn" folder where C# solution "ScLint" with analyzers' source code is placed. Analyzers are located in different folders in Visual Studio solution.

| # | Diagnostic ID | Task | Description | Folder name in project |
| --- | --- | --- | --- | --- |
| 1. | ScLint1 | Avoid direct SQL queries in code |  Code analyzer checks if there are any strings in code with direct SQL queries.<br>Type: warning | DirectSqlCalls | 
| 2. | ScLint2 | Do not use block comments in code | Code is analyzed in respect of use of block comments. There shouldn't be any this kind of comments, only one-line comments are allowed. Code fix provides a possibility to delete it automatically.<br>Type: warning | BlockComments |
| 3. | ScLint3 | Do not use hard coded paths to get Sitecore items | Rule checks variable declarations and return statements to catch all occurrences of obsolete way of referencing items - searching them by providing their paths and using following methods: <i>GetItem</i>, <i>GetRootItem</i>, <i>SelectItems</i>, <i>SelectItemsUsingXPath</i>, <i>SelectSingleItem</i>,  <i>SelectSingleItemUsingXPath</i>. Code analyzer suggests to give items' guids instead of paths and to use other methods to get them.<br>Type: warning | HardCodedPaths |
| 4. | ScLint4 | Do not use GUIDs of unknown items | Rule checks if GUIDs are used as method arguments or as attributes in code - then reports a warning. GUIDs can be provided only while assigning to variables.<br>Type: warning | HardCodedGuids |
| 5. | ScLint5 | Do not use hard coded image paths | Rule checks if paths to media library items are used direct (being not wrapped in variables) in methods as parameters. This approach is reported as a warning since paths should be assigned to variables to suggest which items they are referring to.<br>Type: warning |  HardCodedImagePaths |

## Development

### Code analyzers
C# solution contains separate folder for each diagnostic rule where code analyzers (<em>[rule_name]Analyzer.cs</em>) and code fix providers (<em>[rule_name]CodeFixProvider.cs</em>) are placed.<br>In code analyzer's file there are label strings declared (<em>DiagnosticId, Title, Category</em> variables). <em>Initialize</em> method is for launching analyzing for specific code elements (given as a second parameter in <em>RegisterSyntaxNodeAction</em> method) and <em>AnalyzeNode</em> is a scenario when given code element appeared in code. There should be also a hence call to code fix provider.<br>In code fix provider's file, in <em>RegisterCodeFixesAsync</em> method we should provide an algorithm to fix reported part of code or leave this method empty when e.g. warning report is sufficient and there is no need to change the syntax.

## Disclaimer

Sitecore® is a registered trademark of [Sitecore Corporation](https://www.sitecore.com/).

## Contributing

Your pull requests are very welcome.

### License

This work is [APACHE LICENSE, VERSION 2.0 licensed](./LICENSE).
