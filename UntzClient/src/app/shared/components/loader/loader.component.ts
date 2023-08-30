import { Component } from '@angular/core';
import { LoadingService } from 'src/services/loading.service';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.css']
})
export class LoaderComponent {
  constructor(public loader: LoadingService){}
}
