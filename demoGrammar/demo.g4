grammar demo;


demoLang 
 : 'hello' ID
 ;

ID : [a-z]+ ;
WS : [ \t\r\n]+ -> skip ;