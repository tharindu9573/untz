import { Observable } from "rxjs";

export class Countdown{
    public static setCountDown(endDate: Date): Observable<string>{
        return new Observable<string>(_ => {
            const countDownDate = new Date(endDate).getTime();
            const func = setInterval(() => {
              var now = new Date().getTime();
        
              var distance = countDownDate - now;
        
              var days = Math.floor(distance / (1000 * 60 * 60 * 24));
              var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
              var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
              var seconds = Math.floor((distance % (1000 * 60)) / 1000);
        
              let countDownTime = days.toString().padStart(2, "0") + "DAYS " + hours.toString().padStart(2, "0") + ":" + minutes.toString().padStart(2, "0") + ":" + seconds.toString().padStart(2, "0") + " REMAINING";
        
              if (distance < 0) {
                clearInterval(func);
                countDownTime = "Expired";
              }
              
              _.next(countDownTime);

            }, 1000);
        })
    }
}