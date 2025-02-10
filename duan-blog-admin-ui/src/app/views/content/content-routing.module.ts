import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostComponent } from './posts/post.component';
import { AuthGuard } from '../../shared/auth.guard';
import { PostCategoriesComponent } from './post-categories/post-categories.component';
import { SeriesComponent } from './series/series.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'posts',
    pathMatch: 'full',
  },
  {
    path: 'post-categories',
    component: PostCategoriesComponent,
    data: {
      title: 'Danh mục',
      requiredPolicy: 'Permissions.PostCategories.View',
    },
    canActivate: [AuthGuard],
  },
  {
    path: 'posts',
    component: PostComponent,
    data: {
      title: 'Bài Viết',
      requiredPolicy: 'Permissions.Posts.View',
    },
    canActivate: [AuthGuard],
  },
  {
    path: 'series',
    component: SeriesComponent,
    data: {
      title: 'Series',
      requiredPolicy: 'Permissions.Series.View',
    },
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ContentRoutingModule {}
