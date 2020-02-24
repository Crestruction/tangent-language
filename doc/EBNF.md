statment = (key ":" value newline) [childStatment] | childStatment;
childStatment = newline {tab statment newline};
key = keyword | idenifier | functionName | className;
value = string | number | luaExpression
