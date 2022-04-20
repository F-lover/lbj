#include <reg51.h>
#include <intrins.h>

typedef unsigned char uint8;
typedef unsigned int  uint16;
typedef unsigned long uint32;

uint8 key=0x00;

xdata uint8 row _at_ 0x8001;//键盘的行
xdata uint8 cow _at_ 0x8002;//位选
xdata uint8 led _at_ 0x8004;//段选

code uint8 tab[16] = {0x3f, 0x06, 0x5b, 0x4f, 0x66, 0x6d, 0x7d, 0x07, 0x7f, 0x6f, 0x77, 0x7c, 0x39, 0x5e, 0x79, 0x71};     //断码表
//8个灯
#define w1 0x01				
#define w2 0x02				
#define w3 0x04				
#define w4 0x08				
#define w5 0x10				
#define w6 0x20				
#define w7 0x40		
#define w8 0x80


uint8 wayPoint=1,delay=1;//彩灯控制方式		
uint8 ledPoint=0;		//亮灯方式指针		
uint32 count=0;		//中断计数		
uint8 _key=0;	//键值			

bit flag=0;


void delayMS(uint16 x)   //延时 消除抖动
{  
    uint16 i;
		uint8 j;
    for (i = x; i > 0; i--)
        for (j = 110; j > 0; j--)
            ;
}

void ledDisplay(_val,_cow)  //数码管显示
{
		cow = _cow;
		led = tab[_val+1]; //按键0-9   方式1-10
		delayMS(100);  
		cow = 0x00;
		delayMS(10);
}

uint8 keyScan()      //获取键值
{
		cow=0x00;    	//列全为低电平
		if(row!=0x0f)   //有行变为低电平有键按下
		{
				delayMS(10);  //延时 消除抖动
				cow=0x00;		 
				switch(row){
						case 0x07: key=0x40;break;		//row4
						case 0x0b: key=0x30;break;		//row3
						case 0x0d: key=0x20;break;		//row2
						case 0x0e: key=0x10;break;		//row1
						default: return key;
				} //判断是那一行
				
				//判断是那一列
				cow=0x1f;								//cow1
				if(row!=0x0f) key=key+0x01;
				delayMS(1);
				
				cow=0x2f;								//cow2
				if(row!=0x0f)	key=key+0x02;
				delayMS(1);

				cow=0x37;								//cow3
				if(row!=0x0f)	key=key+0x03;
				delayMS(1);
			
				cow=0x3b;								//cow4
				if(row!=0x0f)	key=key+0x04;
				delayMS(1);
				
				cow=0x3d;								//cow5
				if(row!=0x0f)	key=key+0x05;
				delayMS(1);
				
				cow=0x3e;								//cow6
				if(row!=0x0f) key=key+0x06;
				delayMS(1);
				
				cow=0x00;
				delayMS(1);
				
				while(row!=0x0f) delayMS(10);
				   row=0x0f;
				return key;  	//返回键值 0xmn  m为行 n为列
		}
		return 0x00;
}
uint8 TestKey()             //按键是否摁下
{
   cow= 0;            // 输出线置为0
   return (~row & 0x0f);   // 读入键状态 (高四位不用)
}

code uint8 way[10][10]={
		{1,w1, w2, w3, w4, w5, w6, w7, w8}, 
		{2,w8, w7, w6, w5, w4, w3, w2, w1},		
		{3,w1|w2|w3 ,w2|w3|w4, w3|w4|w5, w4|w5|w6, w5|w6|w7, w6|w7|w8, w7|w8|w1}, 
		{10,w1|w8, w2|w7, w3|w4, w5|w6, w5|w6, w4|w3, w7|w2, w8|w1},        	                    				
		{5,w1,w8,w2,w7,w3,w4, w5, w6},                         
        {8,w1, w3, w5, w7, w2, w4, w6, w8}, 	                               
        {2,w1|w2, w2|w3, w3|w4, w4|w5, w5|w6, w6|w7, w7|w8, w8|w1},                              
		{1,w1, w5, w2, w6, w3, w7, w4, w8},
		{9,w8, w6, w1, w4, w2, w7, w5, w3},
		{3,w5|w6, w1|w8, w2|w7, w4|w5, w3|w4},

};//灯 的10种花样显示

void time0server() 	interrupt 1      //定时器0中断服务程序
{               
                TH0=0x3c;								
		TL0=0xb0;                    //50ms
 		if(count!=0){
		   count--;
                      return;
		}		   
		else{								   
				ledPoint++;             			
				if(ledPoint==9) ledPoint=1;   		
				P1=way[wayPoint][ledPoint];   	
				count=delay;	
                       }

}
void SerialIO0() interrupt 4
{
	
			
	 wayPoint=SBUF;
		
		
		 TI=0;
            
		//R0=1;
}
void start()							//开始				
{
		flag=1;
		wayPoint=0;                     //初始化计数器
		ledPoint=0;
		count=delay;                   //从way的第一列获取中断计数值
		P1=way[wayPoint][ledPoint];    //全灭   
   
		TMOD=0x21;				      //定时器0方式2	
      
		EA=1; 										 	
		ET0=1;									
		TH0=0x3c;	                 //50ms							
		TL0=0xb0;
   
                TH1=0xF3;
                TL1=0xF3;            //波特率2400
                PCON= 0x00;          // SMOD 位清零
	        SCON=0x50;
	        TR1=1;
	        PS=1;
                ES=1;
   
   
		TR0=1;                       //开始计时 
}

void pause()					    //暂停/继续					
{
		TR0=~TR0;              		//计时器状态取反		
}

void stop()							//停止			
{
		P1=0x00;                    //全灭  
		TR0=0;                      //关闭定时器
		flag=0;
}

void main()
{
		while(1)
		{
				if(flag==1){
						ledDisplay(wayPoint,0x01);     //数码管显示按键
				}
				if(TestKey())  {
				_key=keyScan(); 					//获取键值	
				switch(_key){
				                case 0x11:        wayPoint=0;break;
						case 0x21:	wayPoint=1;break;    
						case 0x22:	wayPoint=2;break;
						case 0x23:	wayPoint=3;break;
						case 0x31:	wayPoint=4;break;
						case 0x32:	wayPoint=5;break;
						case 0x33:	wayPoint=6;break;
						case 0x41:	wayPoint=7;break;
						case 0x42:	wayPoint=8;break;
						case 0x43:	wayPoint=9;break;
					    case 0x44:	delay+=3;break;
						case 0x34:	if(delay>2)delay-=2;break;
						case 0x12:	start();break;           
						case 0x13:	pause();break;        
						case 0x14:	stop();break;	         
						default:break;
				}
			     }
		}
}