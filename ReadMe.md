# Bespoke-BNF
> **WIP**

Bespoke-BNF (**BBNF**) is designed to be a more complete and extensible variant of BNF. It's syntax is designed with common conventions, compatibility, and readability in mind while allowing for a more complete, succinct, and descriptive specification of a language's syntax. It is mostly based on the [BNF](https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_form) flavors: [EBNF](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form), [ABNF](https://en.wikipedia.org/wiki/Augmented_Backus%E2%80%93Naur_form) and [XBNF](https://en.wikipedia.org/wiki/Extensible_Backus%E2%80%93Naur_form), but with some additional features and conventions.

## Feature Comparisons
| *Features*                       | **ReGex** | **BNF**                                                | **EBNF**             | **ABNF**                                              | **XBNF**                       | ***BBNF***                                                          |
|----------------------------------|-----------|--------------------------------------------------------|----------------------|-------------------------------------------------------|--------------------------------|---------------------------------------------------------------------|
| **Production Defenition**        | `/y/`     | `x ::= y`                                              | `x ::= y`            | `x = y`                                               | `x = y`                        | `x ::= y` or `x = y` or `x: y`                                      |
| **Sequential Concatenation**     | `xy`      | `x y`                                                  | `x , y`              | `x y`                                                 | `x y`                          | `x y`  or `x , y`                                                   |
| **End of Rule Terminator**       | `/`       | `;`                                                    | `;`                  | `;` or `\n\r` [^ABNF-NLCR]                            | `;`                            | `;`                                                                 |
| **Rule Reference**               | *X*       | `<rule>`                                               | `rule`               | `rule` or `<rule>`                                    | `rule` [^EBNF-CvsL]            | `rule` [^EBNF-CvsL] or `<rule>`                                     |
| **Token Reference**              | *X*       | `<token>`                                              | `token`              | `token` or `<token>`                                  | `TOKEN` [^EBNF-CvsL]           | `TOKEN` [^EBNF-CvsL] or `<TOKEN>`                                   |
| **Character Literal**            | `c`       | `"c"` or `'c'`                                         | `"c"`                | `"c"`                                                 | `"c"`                          | `'c'`                                                               |
| **Unicode Literal**              | `\uHEX`   | *???*                                                  | *???*                | *???*                                                 | *???*                          | `'c'` or `\uHEX`                                                    |
| **String Literal**               | `text`    | `"text"` or `'text'`                                   | `"text"` or `'text'` | `"text"` or `'text'`                                  | `"text"`                       | `"text"`                                                            |
| **Regex Literal**                | `regex`   | *X*                                                    | `/regex/`            | *X*                                                   | `/regex/`                      | `/regex/`                                                           |
| **Decimal Literal**              | `123`     | `123`                                                  | `123`                | `123` or `%d123`                                      | `123`                          | `123`                                                               |
| **Hexidecimal Literal**          | `\xHEX`   | *???*                                                  | *???*                | `%x123` [^ABNF-XHexKey]                               | *???*                          | `\xHEX`                                                             |
| **Octal Literal**                | `\xOCT`   | *???*                                                  | *???*                | `%o123`                                               | *???*                          | `\oOCT`                                                             |
| **Numeric Range**                | *???*     | *X*                                                    | *X*                  | `123-456`                                             | *X*                            | `123-456` or `123..456`                                             |
| **Escape Sequence**              | *???*     | `"\c"` or `'\c'`                                       | `"\c"` or `'\c'`     | `"\c"` or `'\c'`                                      | `"\c"`                         | `\c` or `"\c"` or `'\c'` or `\c`                                    |
| **Comment / Ignored**            | *???*     | `comment` [^BNF-Prose]                                 | `(* comment *)`      | `; comment`                                           | `# comment` or `/* comment */` | `# comment` [^BBNF-CvsT] or `/* comment */`                         |
| **Alternate Choice**             | *???*     | `x \| y` <br> or: (`x = y` and `x = z`) [^BNF-AltSame] | `x \| y`             | `x / y` <br> or: (`x = y` and `x =/ z`) [^ABNF-AltEq] | `x \| y`                       | `x \| y` or `x\|y` [^BBnf-OrPrec]                                   |
| **Not / Inverse**                | *???*     | *X*                                                    | *X*                  | *X*                                                   | *X*                            | `!x` or `x!`                                                        |
| **Unordered Concatenation**      | *???*     | *X*                                                    | *X*                  | *X*                                                   | *X*                            | `{x}`                                                               |
| **Immediate Concatenation**      | *???*     | *X*                                                    | *X*                  | *X*                                                   | `x . y`                        | `x . y`                                                             |
| **Grouping**                     | *???*     | *X*                                                    | `( x y )`            | `( x y )`                                             | `( x y )`                      | `( x y )`                                                           |
| **Optional**                     | *???*     | *X*                                                    | `[x]`                | `[x]` or `*1x`                                        | `x?`                           | `[x]` or `x?` or `?x`                                               |
| **Repeat Zero or More Times**    | *???*     | `s = \| i s`                                           | `{ }`                | `*x`                                                  | `x*`                           | `x*` or `*x`                                                        |
| **Repeat One or More Times**     | *???*     | `s = i \| i s`                                         | `{ }`                | `1*x`                                                 | `x+`                           | `x+` or `+x`                                                        |
| **Repeat Exactly N Times**       | *???*     | *X*                                                    | *X*                  | `Nx`                                                  | *X*                            | `x*N` or `N*x`                                                      |
| **Repeat Between A and B Times** | *???*     | *X*                                                    | *X*                  | `A*Bx`                                                | *X*                            | `x*A-B` or `A..B*x` or `A-Bx` or `token..B*x` etc... [^BBnf-RngOps] |
| **Repeat N or More Times**       | *???*     | *X*                                                    | *X*                  | `N*x`                                                 | *X*                            | `x*N+`  or `N+*x`                                                   |
| **Repeat N or Less Times**       | *???*     | *X*                                                    | *X*                  | `0*Nx`                                                | *X*                            | `x*N-`  or `N-*x`                                                   |
| **Repeated and Comma Separated** | *???*     | *X*                                                    | *X*                  | `#x`                                                  | *X*                            | *X*                                                                 |
| **Special Sequence**             | *???*     | *X*                                                    | *X*                  | `? x ?`                                               | *X*                            | ``` `{key}x` ```                                                    |
| **Prose**                        | *???*     | `comment` [^BNF-Prose]                                 | *X*                  | *X*                                                   | *X*                            | `; prose` [^XBNF-Prose]                                             |
| **Spread Operator**              | *???*     | *X*                                                    | *X*                  | *X*                                                   | *X*                            | `...rule`                                                           |
| **Metadata Tags**                | *???*     | *X*                                                    | *X*                  | *X*                                                   | *X*                            | `#tag` [^BBNF-CvsT]                                                 |
| **Exceptions**                   | *???*     | *X*                                                    | *X*                  | `- error`                                             | *X*                            | `#err`                                                              |

### Table Notes
- *X* means that the feature is not explicitly defined in the given standard.
- *???* means that an investigation still needs to be done to determine the correct syntax.
- All whitespace characters are optional.
- In most[^ABNF-XHexKey] examples `x`, `y`, and `z` are placeholders for any valid expression.
- `c` is a placeholder for any valid character.
- `s` is a placeholder for a valid BNF expression representing some kind of sequence, and `e` is a placeholder for a valid BNF element within that sequence. 
- `123`, `1234`, `N`, `A`, and `B` are placeholders for any valid integer.
- `HEX` is a placeholder for any valid hexadecimal number code.	
- `OCT` is a placeholder for any valid octal number code.	
- `key`, `rule`, `token`, `text`, `tag`, `regex`, `comment`, ...etc are also placeholders, but for their respective types.

### Sources
- https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_form {XBNF}
- https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form {EBNF}
- https://en.wikipedia.org/wiki/Augmented_Backus%E2%80%93Naur_form {ABNF}
- http://www.faqs.org/rfcs/rfc2234.html {ABNF}
- http://www.cs.man.ac.uk/~pjj/bnf/ebnf.html#NOTE {BNF, EBNF, ABNF, XBNF}
- https://marketplace.visualstudio.com/items?itemName=Mai-Lapyst.xbnf {XBNF}
- https://sabnf.com/docs/doc7.0/abnf.html {ABNF}
- https://www.ietf.org/rfc/rfc1945.txt {ABNF}

## Usage

### Defining Grammars
Defining a grammar is done in two main steps: defining tokens and defining rules.
#### Defining Tokens
#### Defining Rules

### Using Grammars

## Syntax

### Comments
#### Inline Comments
Inline comments are defined as starting with either: 
- a double forward slash (`//`),
- or at least one hash symbol followed by a padding character (`#+[ |\t]`) and then the comment text.
```bbnf
// This is an inline comment
# This is another inline comment
#  This is a third inline comment
## This is a fourth inline comment
rule ::= token // This is an inline comment at the end of a line
    ; // make sure you don't comment out the semicolon!
```

### Literals

### Groups

### Operators

### Tags
#### Built-in Tags
BBNF Has a handfull of built-in tags that can be used to provide additional information about tokens, rules, and expressions to the parser and lexer.
These tags are used to provide additional context to the parser and lexer, and can be used to transform the value or modify the behavior of their target rule or token.

##### WhiteSpace Tags
These tags are used by both the lexer and parser to provide additional information about how whitespace should be handled.
- `ws|whitespace`
  : Tag indicating that the token should be treated as general whitespace (breaking or inline).
  - Multiple tokens with this tag can be defined as long as each has a different value.
- `pad|padding`
  : Indicates that the token should represent inline-padding between tokens.
  - If present, the parser will include padding as tokens in the lexical output as well.
  - The value of this token can be empty or can contain the accepted padding characters as a literal of any kind: `"\t"`, `'\t'`, `/\t| {2-4}/` etc. 
  - If the token with this tag is left empty, the default value of: `/\t| /` will be used.
  - Multiple tokens with this tag can be defined as long as each has a different value.
- `nl|newline`
  : Used to indicate that the token should represent newlines. 
  - If present, the parser will include newlines as tokens in the lexical output as well.
  -  The value of this token can be empty or can contain the accepted newline characters as a literal of any kind: `"\n"`, `'\n'`, `/\r|\n/` etc. 
  - If the token with this tag is left empty, the default regex pattern of: `/\r[\n\f]?|[\n\f]\r?/` will be used (This should account for all possible ASCII-Compatible newline character combinations). 
  - Multiple tokens with this tag can be defined as long as each has a different value.
- `ident|indent` 
  : A tag indicating that the token should represent an increase in the current line's indentation when compared to the previous line(s).
  - If present, the parser will include indentations as tokens in the lexical output as well.
  - The value of this token can be empty or can contain the accepted indentation characters as a literal of any kind: `"\t"`, `'\t'`, `/\t| {2-4}/` etc. 
  - If the token with this tag is left empty, the default value of: `/\t| /` will be used.
  - Multiple tokens with this tag can be defined as long as each has a different value.
- `ddent|dedent`
  : Indicates that the token should represent a decrease in the current line's indentation when compared to the previous line(s).
  - An `indent` token must also be defined for this tag to be used.
  - If present, the parser will include dedentations as tokens in the lexical output as well.
  - The value of this token can b empty, or should contain the accepted dedented intentation characters to match to this token as a literal of any kind: `"\t"`, `'\t'`, `/\t| {2-4}/` etc.
  - If the token with this tag is left empty and there is only one: the default value will match the `indent` token's value, otherwise: only one empty `dedent` token can be defined and it's value will be that of any un-handled values defined by the `indent` token.
  - Multiple tokens with this tag can be defined as long as each has a different value.
- `sdent|samedent`
  : Indicates that the token should represent the same indentation as the previous line(s).
  - An `indent` token must also be defined for this tag to be used.
  - If present, the parser will include indentations that match the previous level as tokens in the lexical output as well.
  - The value of this token must be empty.
  - If the token with this tag is left empty, the default value of: `/\t| /` will be used.

##### Lexer Token Tags
These tags are used to provide additional information about tokens to the lexer, or to request the lexer provide additional information about the lexed tokens to the parser.
- `ixd|indexed`
  : Indicates that the token should be indexed by the lexer, with information about it's number of occurrences and positions output to the parser as extra input context.

##### Parser Expression Tags
> WIP

#### Custom Tags
> WIP

## TODO
- Syntax Features
  - [x] update regex literal to use using `/` as delimiters.
  - [x] add block style comments.
  - [x] add other defenition assignment operators.
  - [x] add special sequences.
  - [x] add full repeat syntax.
  - [x] allow all suffixes as prefixes in BBnf.
  - [x] Add Unordered Sequence using: `{}`.
  - [x] Add Spread (`...`) operator as just prefix for templating and splatting.
  - [x] add a general numeric range syntax.
  - [ ] add negative to the numeric literals.
  - [ ] make the numeric literal base type into the literal type.
  - [ ] add plus or minus tailing syntax to repeat.
  - [ ] add 'spaced' boolean to the or (`|`) operator.
  - [ ] investigate and add hexidecimal and octal literals? (or at least tags).
- Tests
  - [ ] Add tests for Creating a set of Tokens and Grammar rules from code
    - [ ] add Token parser tests
    - [ ] add Rule parser tests
- Grammar Parser Implementation
  - [ ] implement each rule's expression parser {0%}
  - [ ] implement comment handling
  - [ ] implement the rule based parser logic, state, etc.
  - [ ] implement Tags
    - [ ] implement the whitespace tags 
    - [ ] implement other built-in tags
      - [ ] indexer tag 
    - [ ] implement custom tags
- Documentation
  - [ ] Finish the usage docs
    - [ ] Finish the quick start guide
  - [ ] Finish the syntax documentation

[^BBNF-CvsT]: **BBNF - Comments vs Tags** Both inline comments and tags in BBNF use the `#` symbol as a prefix. Inline comments require a whitespace charachter after the `#` and before the comment text (spaced), while tags use immediate syntax and require there to be no whitespace after the symbol and before the key (unspaced).
[^EBNF-CvsL]: **EBNF - Capitalization vs Lowercase** In some BNF variants; The capitalization used to reference other productions is significant. Productions for tokens are written in all uppercase, while productions for rules are written in all lowercase.
[^BNF-AltSame]: **BNF - Alternate Same** In pure BNF, alternate choices to the same production can defined by separate rules with the same name. This is not the case in most BNF variants.
[^ABNF-AltEq]: **ABNF - Alternate Equal** In ABNF, alternate choices are defined by separate rules with the same name, but with the `=` and `=/` operators. 
[^ABNF-NLCR]: **ABNF - Newline and Carriage Return** In ABNF, the end of a rule can be terminated by either a semicolon (`;`) or a newline that begins with no indentation (`\n\r`).
[^BNF-Prose]: **BNF - Non-Delimited Literals as Prose** In BNF, comments are not explicitly defined, but any text that is not part of a production is often considered prose... and can be a comment or human readable description of 'extraneous' logic that is not achievable by BNF's basic syntax and rules.
[^XBNF-Prose]: **XBNF - Prose Comments** Unlike BNF, XBNF differentiates between common documentation comments and 'prose'. Prose can only come after a semicolon at the end of a rule and before the next newline. While comments are entirely descriptive and could be ignored; Prose is human readable plain text that contains extra logic for a rule which could not be handled by BBnf's syntax or easily described using custom tags.
[^ABNF-XHexKey]: **ABNF - X Hex Keyword** In this ABNF example; the `x` is not a placeholder for a production, but is instead a literal part of the required syntax.
[^BBnf-RngOps]: **BBnf - Range Options** In BBNF; a range is defined using any combination of two Numbers [Ex; `123`, `-4`], or Characters [Ex; `a`, `b`], or Token types that resolve to a any of the previously mentioned types (`token..543`).
[^BBnf-OrPrec]: **BBnf - Alternalte/Or Precidence** In BBnf, you can use the spacing of the or/alternate operator (`|`) to specify the precidence of multiple or operations. The Alternate/Or operation has low precidence when spaced (`x | y`), and higher precidence when unspaced (`x|y`).