import {
  Injectable,
  Optional
} from '@angular/core';

import {
  Comment,
  WebApi
} from '../models';

import {
  QueryGeneratorService,
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
    private generator: QueryGeneratorService,
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) {
    this.api = new WebApi(http, config, snacker, 'comment');
  }

  queryComments = (postId?: number) =>
    this.generator.generateSource<Comment>(
      `dateCreated`,
      postId
        ? `comment/queryComments/${postId}`
        : null
    );

  queryAuthoredComments = (authorId?: number) =>
    this.generator.generateSource<Comment>(
      `dateCreated`,
      authorId
        ? `comment/queryAuthoredComments/${authorId}`
        : null
    );

  private comments = new BehaviorSubject<Comment[]>(null);
  comments$ = this.comments.asObservable();

  getSubComments = (commentId: number) =>
    this.api.assign(
      `getSubComments/${commentId}`,
      this.comments
    );

  private comment = new BehaviorSubject<Comment>(null);
  comment$ = this.comment.asObservable();

  getComment = (commentId: number) =>
    this.api.assign(
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
