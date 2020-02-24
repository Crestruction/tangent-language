`EBNF
newline ::= '\r'?'\n';    
comment ::= '#' string newline;    
tab ::= '\t' | "    ";

statment ::= (key ':' value newline) (childStatment)? | childStatment;    
childStatment ::= newline (tab statment newline)*;         

key ::= keyword | functionName | className;    
value ::= string | luaExpression;    

name ::= (anyChar -('.'|':'|'$'))+;
functionName ::= '$' name;
className ::= name;
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
string ::= (anyChar - newline)+;
    
`
