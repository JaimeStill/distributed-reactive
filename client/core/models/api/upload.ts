import { Post } from './post';
import { Topic } from './topic';
import { User } from './user';

export interface Upload {
  id: number;
  type: string;
  url: string;
  path: string;
  file: string;
  name: string;
  fileType: string;
  size: number;
}

export interface PostUpload extends Upload {
  postId: number;
  post?: Post;
}

export interface TopicImage extends Upload {
  topicId: number;
  topic?: Topic;
}

export interface UserImage extends Upload {
  userId: number;
  user?: User;
}
