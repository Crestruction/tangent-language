### basics
```EBNF
anyChar := K'^((?!/\*|\*/|\n)[\s\S])*$'
tab ::= '\t' | '    ';
newline ::= '\r\n' | '\r' | '\n';
name ::= (anyChar -('.'|':'|'$'))+;   
string ::= (anyChar - newline)+;   
```

### comments

```EBNF
comment ::= '#' string newline;  
```

### keywords
```EBNF
keyword ::=    
    userCustumKeyword |   
    "call" |   
    "log" |   
    "if" |   
    "cond" |   
    "true" |   
    "false" |   
    "switch" |   
    "case" |   
    "prog" |   
    "while" |   
    "eval";   
```

### statements

```EBNF
functionName ::= '$' name;   
className ::= name;   

key ::= keyword | functionName | className;    
value ::= string | luaExpression;  


statment ::= 
    (key ":" value newline childStatment) |     
    (key ":" value newline) |    
    childStatment;     
childStatment ::= newline (tab statment newline)*;         

document ::= statment+;    




 
    
```
