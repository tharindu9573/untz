import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {

  public shouldLoad = new BehaviorSubject<boolean>(false);
  isLoading$ = this.shouldLoad.asObservable();
  constructor() { }
  setLoading(flag: boolean){
    this.shouldLoad.next(flag);
  }
}
