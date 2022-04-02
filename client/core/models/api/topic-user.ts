import { Topic } from './topic';
import { User } from './user';

export interface TopicUser {
  id: number;
  topicId: number;
  userId: number;
  isAdmin: boolean;
  isBanned: boolean;

  topic?: Topic;
  user?: User;
}
