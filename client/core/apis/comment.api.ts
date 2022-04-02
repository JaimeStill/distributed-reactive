import {
  Injectable,
  Optional
} from '@angular/core';

import {
  Comment,
  WebApi
} from '../models';

import {
  SnackerService,
  SyncService
} from '../services';

import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { ServerConfig } from '../config';

@Injectable({
  providedIn: 'root'
})
export class CommentApi {
  api: WebApi;

  constructor(
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) {
    this.api = new WebApi(http, config, snacker, 'comment');
  }

  private comments = new BehaviorSubject<Comment[]>(null);
  comments$ = this.comments.asObservable();

  getSubComments = (commentId: number) =>
    this.api.assign<Comment[]>(
      `getSubComments/${commentId}`,
      this.comments
    );

  private comment = new BehaviorSubject<Comment>(null);
  comment$ = this.comment.asObservable();

  getComment = (commentId: number) =>
    this.api.assign<Comment>(
      `getComment/${commentId}`,
      this.comment
    );

  saveComment = (comment: Comment) =>
    this.api.send<Comment, Comment>(
      'saveComment',
      comment,
      () => this.snacker.sendSuccessMessage('Comment saved successfully')
    );

  removeComment = (comment: Comment) =>
    this.api.resolve(
      'removeComment',
      comment,
      () => this.snacker.sendSuccessMessage('Comment removed successfully')
    );
}
