/*
 * Copyright (c) 2007 Josh Bush (digitalbush.com)
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:

 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE. 
 */
 
/*
 * Version: 1.1.1
 * Release: 2007-10-02
 */ 

//Predefined character definitions
var charMap = {'9':"[0-9]",'a':"[A-Za-z]", '*':"[A-Za-z0-9]"};
Element.addMethods({

	mask: function(element, mask, settings) 
	{	
			settings = Object.extend
		(
		    {
			    placeholder: "_",
			    completed: null
		    }, 
		        settings
		);
		
	    element = $(element);
		var input=element;
		var buffer=new Array(mask.length);
	    var locked=new Array(mask.length);
	    var valid=false;   
	    var ignore=false;  			//Variable for ignoring control keys
	    var firstNonMaskPos=null; 
	    var re = new RegExp("^"+ mask.toArray().collect( function(c,i){c=c||mask.charAt(i);	return charMap[c]||((/[A-Za-z0-9]/.test(c)?"":"\\")+c);}).join('')+"$");
		
		//Build buffer layout from mask & determine the first non masked character			
	    $A(mask).each(function(letter, count)
	    {
		    letter=letter||mask.charAt(count);
		    locked[count]=(charMap[letter]==null);
			
		    buffer[count]=locked[count]?letter:settings.placeholder;									
		    if(!locked[count] && firstNonMaskPos==null)
			    firstNonMaskPos=count;
	    }); 

	    Event.observe(element, "focus", focusEvent); 
	    Event.observe(element, "blur", onBlur); 
	    Event.observe(element, "keydown", onKeydown); 
	    Event.observe(element, "keypress", onKeypress); 
		
		//Paste events for IE and Mozilla thanks to Kristinn Sigmundsson
	    if (Prototype.Browser.IE) 
		    Event.observe(element, "onpaste", onIEPaste);                     
	    else if (Prototype.Browser.Gecko)
		    Event.observe(element, "input", onFFInput); 
		
		function onBlur(evnt){checkVal(evnt);};
	    function onKeydown(evnt){ keydownEvent(evnt);};
	    function onKeypress(evnt){ keypressEvent(evnt);};
	    function onIEPaste(evnt){setTimeout(checkVal(evnt),0); Event.stop(evnt); };
		function onFFInput (evnt){checkVal(evnt); Event.stop(evnt); return false;};
			
		//Helper Functions for Caret positioning
        function getCaretPosition(ctl){
            var res = {begin: 0, end: 0 };
            if (document.selection && document.selection.createRange){
                var range = document.selection.createRange();			
                res.begin = 0 - range.duplicate().moveStart('character', -100000);
                res.end = res.begin + range.text.length;
            }
            else if (ctl.setSelectionRange){
                res.begin = ctl.selectionStart;
                res.end = ctl.selectionEnd;
            } 
            return res;
        };
        	   
	   function setCaretPosition(ctl, pos){
            if (typeof(ctl.createTextRange) == "object"){
                var range = ctl.createTextRange();
                range.collapse(true);
                range.moveEnd('character', pos);
                range.moveStart('character', pos);
                range.select();
            }
            else if(ctl.setSelectionRange){
                ctl.setSelectionRange(pos,pos);
            } 
            
        };
	    
	    /* Event Functions */
	    function focusEvent(e){					
		    checkVal(e);
		    
		    var cntrl;
		    if(element != null && element != 'undefined')
		        cntrl = element;
		    else
		        cntrl = e.target;
		        
		    writeBuffer();
		    setTimeout(function(){
			    setCaretPosition(cntrl,valid?mask.length:firstNonMaskPos);
		    },0);
		    
		    Event.stop(item);
	    };
        	   
        	    	    
	    function keydownEvent(e){
	    
	    	if(element != null && element != 'undefined')
		        cntrl = element;
		    else
		        cntrl = e.target;
		        
		    var pos=getCaretPosition(cntrl);													
		    var k = e.keyCode;
		    ignore=(k < 16 || (k > 16 && k < 32 ) || (k > 32 && k < 41));
			
		    //delete selection before proceeding
		    if((pos.begin-pos.end)!=0 && (!ignore || k==8 || k==46)){
			    clearBuffer(pos.begin,pos.end);
		    }	
		    //backspace and delete get special treatment
		    if(k==8){//backspace					
			    while(pos.begin-->=0){
				    if(!locked[pos.begin]){								
					    buffer[pos.begin]=settings.placeholder;
					    if(Prototype.Browser.Opera){
						    //Opera won't let you cancel the backspace, so we'll let it backspace over a dummy character.								
						    s=writeBuffer();
						    input.value = s.substring(0,pos.begin)+" "+s.substring(pos.begin);
						    setCaretPosition(cntrl,pos.begin+1);
					    }else{
						    writeBuffer();
						    setCaretPosition(cntrl,Math.max(firstNonMaskPos,pos.begin));
					    }									
					    return false;								
				    }
			    }						
		    }else if(k==46){//delete
			    clearBuffer(pos.begin,pos.begin+1);
			    writeBuffer();
			    setCaretPosition(cntrl,Math.max(firstNonMaskPos,pos.begin));
			    return false;
		    }else if (k==27){//escape
			    clearBuffer(0,mask.length);
			    writeBuffer();
			    setCaretPosition(cntrl,firstNonMaskPos);
			    return false;
		    }		
		    Event.stop(e);							
	    };
    			
	    function keypressEvent(e){					
		    if(ignore){
			    ignore=false;
			    return;
		    }
		    
		    if(element != null && element != 'undefined')
		        cntrl = element;
		    else
		        cntrl = e.target;
		        
		    e=e||window.event;
		    var k=e.charCode||e.keyCode||e.which;
		    var pos=getCaretPosition(cntrl);					
							
		    if(e.ctrlKey || e.altKey){//Ignore
			    return true;
		    }else if ((k>=41 && k<=122) ||k==32 || k>186){//typeable characters
			    var p=seekNext(pos.begin-1);					
			    if(p<mask.length){
				    if(new RegExp(charMap[mask.charAt(p)]).test(String.fromCharCode(k))){
					    buffer[p]=String.fromCharCode(k);									
					    writeBuffer();
					    var next=seekNext(p);
					    setCaretPosition(cntrl,next);
					    if(settings.completed && next == mask.length)
						    settings.completed.call(input);
				    }				
			    }
		    }			
		    Event.stop(e);
		    return false;				
	    };
    			
	    /*Helper Methods*/
	    function clearBuffer(start,end){
		    for(var i=start;i<end;i++){
			    if(!locked[i])
				    buffer[i]=settings.placeholder;
		    }				
	    };
		
	    function writeBuffer(){				
		    return input.value = buffer.join('');				
	    };
		
	    function checkVal(item){	
		    //try to place charcters where they belong
		    
		    var test;
		    if(element != null && element != 'undefined')
		        test = element.value;
		    else
		        test = item.target.value;
		        
		    var pos=0;
		    for(var i=0;i<mask.length;i++){
			    if(!locked[i]){
				    while(pos++<test.length){
					    //Regex Test each char here.
					    var reChar=new RegExp(charMap[mask.charAt(i)]);
					    if(test.charAt(pos-1).match(reChar)){
						    buffer[i]=test.charAt(pos-1);
						    break;
					    }									
				    }
			    }
		    }
		    var s=writeBuffer();
		    
		    if(test.length == mask.length)
		    {
		        if(!s.match(re)){	
		            input.value = "";	
			        clearBuffer(0,mask.length);
			        valid=false;
		        }else
			        valid=true;
		    }	    
			else if (test.length == 0)
		    {
		        input.value = "";	
		        clearBuffer(0,mask.length);
		        valid=false;
	        }
	    };
		
	    function seekNext(pos){				
		    while(++pos<mask.length){					
			    if(!locked[pos])
				    return pos;
		    }
		    return mask.length;
	    };
	    checkVal(element);//Perform initial check for existing values
		return $(element);
    }
});

