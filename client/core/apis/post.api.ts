import {
  Injectable,
  Optional
} from '@angular/core';

import {
  Post,
  PostLink,
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
export class PostApi {
  api: WebApi;

  constructor(
    private generator: QueryGeneratorService,
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) {
    this.api = new WebApi(http, config, snacker, 'post');
  }

  //#region Post

  queryPosts = () =>
    this.generator.generateSource<Post>(
      `dateCreated`,
      `post/queryPosts`
    );

  queryTopicPosts = (topicId?: number) =>
    this.generator.generateSource<Post>(
      `dateCreated`,
      topicId
        ? `topic/queryTopicPosts/${topicId}`
        : null
    );

  queryPublishedAuthorPosts = (authorId?: number) =>
    this.generator.generateSource<Post>(
      `dateCreated`,
      authorId
        ? `topic/queryPublishedAuthorPosts/${authorId}`
        : null
    );

  queryUnpublishedAuthorPosts = (authorId?: number) =>
    this.generator.generateSource<Post>(
      `dateCreated`,
      authorId
        ? `topic/queryUnpublishedAuthorPosts/${authorId}`
        : null
    );

  private post = new BehaviorSubject<Post>(null);
  post$ = this.post.asObservable();

  getPost = (url: string) =>
    this.api.assign(
      `getPost/${url}`,
      this.post
    );

  savePost = (post: Post) =>
    this.api.send<Post, Post>(
      `savePost`,
      post,
      () => this.snacker.sendSuccessMessage(`Post successfully saved`)
    );

  togglePostPublished = (post: Post) =>
    this.api.resolve(
      `togglePostPublished`,
      post,
      () => this.snacker.sendSuccessMessage(`Post successfully ${post.isPublished ? 'retracted' : 'published'}`)
    );

  removePost = (post: Post) =>
    this.api.resolve(
      `removePost`,
      post,
      () => this.snacker.sendSuccessMessage(`Post successfully removed`)
    );

  //#endregion

  //#region PostLink

  private links = new BehaviorSubject<PostLink[]>(null);
  links$ = this.links.asObservable();

  getPostLinks = (postId: number) =>
    this.api.assign(
      `getPostLinks/${postId}`,
      this.links
    );

  savePostLink = (link: PostLink) =>
    this.api.send<PostLink, PostLink>(
      `savePostLink`,
      link,
      () => this.snacker.sendSuccessMessage(`Post Link successfully saved`)
    );

  removePostLink = (link: PostLink) =>
    this.api.resolve(
      `removePostLink`,
      link,
      () => this.snacker.sendSuccessMessage(`Post Link successfully removed`)
    );

  //#endregion
}
