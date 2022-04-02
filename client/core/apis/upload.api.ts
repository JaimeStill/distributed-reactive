import {
  Injectable,
  Optional
} from '@angular/core';

import {
  BehaviorSubject,
  Observable
} from 'rxjs';

import {
  PostUpload,
  TopicImage,
  UserImage,
  WebApi
} from '../models';

import {
  SnackerService,
  SyncService
} from '../services';

import { HttpClient } from '@angular/common/http';
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
    this.api = new WebApi(http, config, snacker, 'upload');
    this.defaultTopicImage$ = this.api.getUrl('getDefaultTopicImage');
    this.defaultUserImage$ = this.api.getUrl('getDefaultUserImage');
  }

  //#region PostUpload

  private postUploads = new BehaviorSubject<PostUpload[]>(null);
  postUploads$ = this.postUploads.asObservable();

  getPostUploads = (postId: number) =>
    this.api.assign(
      `getPostUploads/${postId}`,
      this.postUploads
    );

  createPostUploads = (postId: number, form: FormData) =>
    this.api.resolve(
      `createPostUploads/${postId}`, form,
      () => this.snacker.sendSuccessMessage('Post uploads successfully saved')
    );

  removePostUpload = (upload: PostUpload) =>
    this.api.resolve(
      'removePostUpload', upload,
      () => this.snacker.sendSuccessMessage('Post upload successfully removed')
    );

  //#endregion

  //#region TopicImage

  private topicImage = new BehaviorSubject<TopicImage>(null);
  topicImage$ = this.topicImage.asObservable();

  getTopicImage = (topicId: number) =>
    this.api.assign(
      `getTopicImage/${topicId}`,
      this.topicImage
    );

  uploadTopicImage = (topicId: number, form: FormData) =>
    this.api.resolve(
      `uploadTopicImage/${topicId}`, form,
      () => this.snacker.sendSuccessMessage('Topic image successfully saved')
    );

  removeTopicImage = (topicId: number) =>
    this.api.action(
      `removeTopicImage/${topicId}`,
      () => this.snacker.sendSuccessMessage('Topic image successfully removed')
    );

  //#endregion

  //#region UserImage

  private userImage = new BehaviorSubject<UserImage>(null);
  userImage$ = this.userImage.asObservable();

  getUserImage = (userId: number) =>
    this.api.assign(
      `getUserImage/${userId}`,
      this.userImage
    );

  uploadUserImage = (userId: number, form: FormData) =>
    this.api.resolve(
      `uploadUserImage/${userId}`, form,
      () => this.snacker.sendSuccessMessage(`User image successfully saved`)
    );

  removeUserImage = (userId: number) =>
    this.api.action(
      `removeUserImage/${userId}`,
      () => this.snacker.sendSuccessMessage('User image successfully removed')
    );

  //#endregion
}
