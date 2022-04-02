import {
  Injectable,
  Optional
} from '@angular/core';

import {
  Comment,
  CommentVote,
  Post,
  PostVote,
  Vote,
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
export class VoteApi {
  api: WebApi;

  constructor(
    private generator: QueryGeneratorService,
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) {
    this.api = new WebApi(http, config, snacker, 'vote');
  }

  //#region CommentVote

  queryUpvotedComments = (voterId?: number) =>
    this.generator.generateSource<Comment>(
      `dateCreated`,
      voterId
        ? `vote/queryUpvotedComments/${voterId}`
        : null
    );

  queryDownvotedComments = (voterId?: number) =>
    this.generator.generateSource<Comment>(
      `dateCreated`,
      voterId
        ? `vote/queryDownvotedComments/${voterId}`
        : null
    );

  private commentUpvotes = new BehaviorSubject<CommentVote[]>(null);
  oommentUpvotes$ = this.commentUpvotes.asObservable();

  getCommentUpvotes = (commentId: number) =>
    this.api.assign(
      `getCommentUpvotes/${commentId}`,
      this.commentUpvotes
    );

  private commentDownvotes = new BehaviorSubject<CommentVote[]>(null);
  commentDownvotes$ = this.commentDownvotes.asObservable();

  getCommentDownvotes = (commentId: number) =>
    this.api.assign(
      `getCommentDownvotes/${commentId}`,
      this.commentDownvotes
    );

  castCommentVote = (vote: CommentVote) =>
    this.api.send<CommentVote, CommentVote>(
      `castCommentVote`,
      vote
    );

  removeCommentVote = (vote: CommentVote) =>
    this.api.resolve(
      `removeCommentVote`,
      vote
    );

  //#endregion

  //#region PostVote

  queryUpvotedPosts = (voterId?: number) =>
    this.generator.generateSource<Post>(
      `dateCreated`,
      voterId
        ? `vote/queryUpvotedPosts/${voterId}`
        : null
    );

  queryDownvotedPosts = (voterId?: number) =>
    this.generator.generateSource<Post>(
      `dateCreated`,
      voterId
        ? `vote/queryDownvotedPosts/${voterId}`
        : null
    );

  private postUpvotes = new BehaviorSubject<PostVote[]>(null);
  postUpvotes$ = this.postUpvotes.asObservable();

  getPostUpvotes = (postId: number) =>
    this.api.assign(
      `getPostUpvotes/${postId}`,
      this.postUpvotes
    );

  private postDownvotes = new BehaviorSubject<PostVote[]>(null);
  postDownvotes$ = this.postDownvotes.asObservable();

  getPostDownvotes = (postId: number) =>
    this.api.assign(
      `getPostDownvotes/${postId}`,
      this.postDownvotes
    );

  castPostVote = (vote: PostVote) =>
    this.api.send<PostVote, PostVote>(
      `castPostVote`,
      vote
    );

  removePostVote = (vote: PostVote) =>
    this.api.resolve(
      `removePostVote`,
      vote
    );

  //#endregion
}
