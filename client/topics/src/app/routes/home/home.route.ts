import { Component } from '@angular/core';
import { UploadApi } from 'core';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [UploadApi]
})
export class HomeRoute {
  constructor(
    public uploadApi: UploadApi
  ) { }
}
