import { Post } from './post';

export interface PostLink {
  id: number;
  postId: number;
  text: string;
  url: string;

  post?: Post;
}
