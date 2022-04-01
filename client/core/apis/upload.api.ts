import {
  Injectable,
  Optional
} from '@angular/core';

import {
  BehaviorSubject,
  Observable
} from 'rxjs';

import {
  Post,
  PostUpload,
  Topic,
  TopicImage,
  User,
  UserImage,
  WebApi
} from '../models';

import { HttpClient } from '@angular/common/http';
import { SnackerService } from '../services';
import { SyncService } from '../services';
import { ServerConfig } from '../config';

@Injectable({
  providedIn: 'root'
})
export class UploadApi {
  api: WebApi;
  defaultTopicImage$: Observable<string>;
  defaultUserImage$: Observable<string>;

  constructor(
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) {
    this.api = new WebApi(http, config, 'upload');
    this.defaultTopicImage$ = this.api.streamUrl('getDefaultTopicImage');
    this.defaultUserImage$ = this.api.streamUrl('getDefaultUserImage');
  }

  //#region PostUpload

  //#endregion

  //#region TopicImage

  //#endregion

  //#region UserImage

  //#endregion
}
