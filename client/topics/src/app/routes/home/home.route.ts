import {
  Component,
  OnInit
} from '@angular/core';

import {
  CommentApi,
  UploadApi
} from 'core';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [UploadApi]
})
export class HomeRoute implements OnInit {
  constructor(
    public commentApi: CommentApi,
    public uploadApi: UploadApi
  ) { }

  ngOnInit(): void {
    this.commentApi.getSubComments(1);
  }
}
