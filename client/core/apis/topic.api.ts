import {
  Injectable,
  Optional
} from '@angular/core';

import {
  Topic,
  TopicUser,
  User,
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
export class TopicApi {
  api: WebApi;

  constructor(
    private generator: QueryGeneratorService,
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) {
    this.api = new WebApi(http, config, snacker, 'topic');
  }

  //#region Topic

  private image = new BehaviorSubject<string>(null);
  image$ = this.image.asObservable();

  getTopicImage = (topicId: number) =>
    this.api.assignUrl(
      `getTopicImage/${topicId}`,
      this.image
    );

  queryTopics = () =>
    this.generator.generateSource<Topic>(
      `name`,
      `topic/queryTopics`
    );

  queryOwnerTopics = (ownerId?: number) =>
    this.generator.generateSource<Topic>(
      `title`,
      ownerId
        ? `topic/queryOwnerTopics/${ownerId}`
        : null
    );

  private topic = new BehaviorSubject<Topic>(null);
  topic$ = this.topic.asObservable();

  getTopic = (url: string) =>
    this.api.assign(
      `getTopic/${url}`,
      this.topic
    );

  verifyTopic = (topic: Topic) =>
    this.api.send<Topic, boolean>(
      `verifyTopic`,
      topic
    );

  saveTopic = (topic: Topic) =>
    this.api.send<Topic, number>(
      `saveTopic`,
      topic,
      () => this.snacker.sendSuccessMessage(`Topic successfully saved`)
    );

  removeTopic = (topic: Topic) =>
    this.api.resolve(
      `removeTopic`,
      topic,
      () => this.snacker.sendSuccessMessage(`Topic successfully removed`)
    );

  //#endregion

  //#region TopicUser

  queryTopicUsers = (topicId?: number) =>
    this.generator.generateSource<User>(
      `lastName`,
      topicId
        ? `topic/queryTopicUsers/${topicId}`
        : null
    );

  queryAvailableTopics = (userId?: number) =>
    this.generator.generateSource<Topic>(
      `name`,
      userId
        ? `topic/queryAvailableTopics/${userId}`
        : null
    );

  queryUserTopics = (userId?: number) =>
    this.generator.generateSource<TopicUser>(
      `name`,
      userId
        ? `topic/queryUserTopics/${userId}`
        : null
    );

  saveTopicUser = (user: TopicUser) =>
    this.api.send<TopicUser, TopicUser>(
      `saveTopicUser`,
      user,
      () => this.snacker.sendSuccessMessage(`${user.user?.displayName} is now a member of ${user.topic?.name}`)
    );

  toggleAdmin = (user: TopicUser) =>
    this.api.resolve(
      `toggleAdmin`,
      user,
      () => this.snacker.sendSuccessMessage(`Admin permissions for ${user.user?.displayName} have been ${user.isAdmin ? 'revoked' : 'granted'}`)
    );

  toggleBanned = (user: TopicUser) =>
    this.api.resolve(
      'toggleBanned',
      user,
      () => this.snacker.sendSuccessMessage(`${user.user?.displayName} has ${user.isBanned ? 'restored access to' : 'been banned from'} ${user.topic?.name}`)
    );

  removeTopicUser = (user: TopicUser) =>
    this.api.resolve(
      `removeTopicUser`,
      user,
      () => this.snacker.sendSuccessMessage(`${user.user?.displayName} is no longer a member of ${user.topic?.name}`)
    );

  //#endregion
}
