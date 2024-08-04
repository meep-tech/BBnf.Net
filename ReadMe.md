# Bespoke-BNF
Bespoke-BNF (**BBNF**) is designed to be a more complete and extensible variant of BNF. It's syntax is designed with common conventions, compatibility, and readability in mind while allowing for a more complete and succinct specification of a language's syntax. It is mostly based on the [BNF](https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_form) flavors: [EBNF](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form), [ABNF](https://en.wikipedia.org/wiki/Augmented_Backus%E2%80%93Naur_form) and [XBNF](https://en.wikipedia.org/wiki/Extensible_Backus%E2%80%93Naur_form), but with some additional features and conventions.

## Comparison
| *Feature*                        | **BNF**                                                | **EBNF**             | **ABNF**                                              | **XBNF**                       | ***BBNF***                                  |
|----------------------------------|--------------------------------------------------------|----------------------|-------------------------------------------------------|--------------------------------|---------------------------------------------|
| **Definition Assignment**        | `x ::= y`                                              | `x ::= y`            | `x = y`                                               | `x = y`                        | `x ::= y`                                   |
| **Sequential Concatenation**     | `x y`                                                  | `x , y`              | `x y`                                                 | `x y`                          | `x y`  or `x , y`                           |
| **End of Rule Terminator**       | `;`                                                    | `;`                  | `;` or `\n\r` [^ABNF-NLCR]                            | `;`                            | `;`                                         |
| **Rule Reference**               | `<rule>`                                               | `rule`               | `rule` or `<rule>`                                    | `rule` [^EBNF-CvsL]            | `rule` [^EBNF-CvsL] or `<rule>`             |
| **Token Reference**              | `<token>`                                              | `token`              | `token` or `<token>`                                  | `TOKEN` [^EBNF-CvsL]           | `TOKEN` [^EBNF-CvsL] or `<TOKEN>`           |
| **Character Literal**            | `"x"` or `'x'`                                         | `"x"`                | `"x"`                                                 | `"x"`                          | `'x'`                                       |
| **Terminal Literal**             | `"text"` or `'text'`                                   | `"text"` or `'text'` | `"text"` or `'text'`                                  | `"text"`                       | `"text"`                                    |
| **Numeric Literal**              | `123`                                                  | `123`                | `123`                                                 | `123`                          | `123`                                       |
| **Numeric Range**                | *X*                                                    | *X*                  | `123-456`                                             | *X*s                           | `123-456`                                   |
| **Escape Sequence**              | `"\x"` or `'\x'`                                       | `"\x"` or `'\x'`     | `"\x"` or `'\x'`                                      | `"\x"`                         | `\x` or `"\x"` or `'\x'`                    |
| **Comment / Ignored**            | *X*                                                    | `(* comment *)`      | `; comment`                                           | `# comment` or `/* comment */` | `# comment` [^BBNF-CvsT] or `/* comment */` |
| **Alternate Choice**             | `x \| y` <br> or: (`x = y` and `x = z`) [^BNF-AltSame] | `x \| y`             | `x / y` <br> or: (`x = y` and `x =/ z`) [^ABNF-AltEq] | `x \| y`                       | `x \| y`                                    |
| **Not / Inverse**                | *X*                                                    | *X*                  | *X*                                                   | *X*                            | `!x` or `x!`                                |
| **Unordered List**               | *X*                                                    | *X*                  | *X*                                                   | *X*                            | `{x}`                                       |
| **Immediate Concatenation**      | *X*                                                    | *X*                  | *X*                                                   | `x . y`                        | `x . y`                                     |
| **Grouping**                     | *X*                                                    | `( x y )`            | `( x y )`                                             | `( x y )`                      | `( x y )`                                   |
| **Optional**                     | *X*                                                    | `[x]`                | `[x]` or `*1x`                                        | `x?`                           | `[x]` or `x?`                               |
| **Repeat Zero or More Times**    | *X*                                                    | `{ }`                | `*x`                                                  | `x*`                           | `x*` or `*x`                                |
| **Repeat One or More Times**     | *X*                                                    | `{ }`                | `1*x`                                                 | `x+`                           | `x+` or `+x`                                |
| **Repeat Exactly N Times**       | *X*                                                    | *X*                  | `Nx`                                                  | *X*                            | `x*N` or `N*x`                              |
| **Repeat Between A and B Times** | *X*                                                    | *X*                  | `A*Bx`                                                | *X*                            | `x*A-B` or `A-B*x`                          |
| **Repeat N or More Times**       | *X*                                                    | *X*                  | `N*x`                                                 | *X*                            | `x*N+`  or `N+*x`                           |
| **Repeat N or Less Times**       | *X*                                                    | *X*                  | `0*Nx`                                                | *X*                            | `x*N-`  or `N-*x`                           |
| **Metadata Tags**                | *X*                                                    | *X*                  | *X*                                                   | *X*                            | `#tag` [^BBNF-CvsT]                         |

### Notes
- All whitespace characters are optional.
- `x`, `y`, and `z` are placeholders for any valid BNF expression. 
- `123`, `N`, `A`, and `B` are placeholders for any valid integer.
- *X* means that the feature is not explicitly defined in the given standard.
- `rule`, `token`, `text`, `tag`, and `comment` are also placeholders, but for their respective types.

## Syntax

## Usage

## TODO
- [ ] Test allowing all suffixes as prefixes in BBnf.

[^BBNF-CvsT]: **BBNF - Comments vs Tags** Both inline comments and tags in BBNF use the `#` symbol as a prefix. Inline comments require a whitespace charachter after the `#` and before the comment text (spaced), while tags use immediate syntax and require there to be no whitespace after the symbol and before the key (unspaced).
[^EBNF-CvsL]: **EBNF - Capitalization vs Lowercase** In some BNF variants; The capitalization used to reference other productions is significant. Productions for tokens are written in all uppercase, while productions for rules are written in all lowercase.
[^BNF-AltSame]: **BNF - Alternate Same** In pure BNF, alternate choices to the same production can defined by separate rules with the same name. This is not the case in most BNF variants.
[^ABNF-AltEq]: **ABNF - Alternate Equal** In ABNF, alternate choices are defined by separate rules with the same name, but with the `=` and `=/` operators. 
[^ABNF-NLCR]: **ABNF - Newline and Carriage Return** In ABNF, the end of a rule can be terminated by either a semicolon (`;`) or a newline that begins with no indentation (`\n\r`).