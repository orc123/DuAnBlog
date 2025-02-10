import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { IconModule } from '@coreui/icons-angular';
import { ChartjsModule } from '@coreui/angular-chartjs';
import { ContentRoutingModule } from './content-routing.module';
import { PostCategoriesComponent } from './post-categories/post-categories.component';
import { PostCategoryDetailComponent } from './post-categories/post-category-detail/post-category-detail.component';
import { DuanSharedModule } from '../../shared/modules/duan-shared.module';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { PanelModule } from 'primeng/panel';
import { BlockUIModule } from 'primeng/blockui';
import { PaginatorModule } from 'primeng/paginator';
import { BadgeModule } from 'primeng/badge';
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';
import { KeyFilterModule } from 'primeng/keyfilter';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { ImageModule } from 'primeng/image';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { EditorModule } from 'primeng/editor';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PostActivityLogsComponent } from './posts/post-activity-logs/post-activity-logs.component';
import { PostDetailComponent } from './posts/post-detail/post-detail.component';
import { PostReturnReasonComponent } from './posts/post-return-reason/post-return-reason.component';
import { PostSeriesComponent } from './posts/post-series/post-series.component';
import { SeriesComponent } from './series/series.component';
import { SeriesDetailComponent } from './series/series-detail/series-detail.component';
import { SeriesPostsComponent } from './series/series-posts/series-posts.component';
import { PostComponent } from './posts/post.component';

@NgModule({
  imports: [
    ContentRoutingModule,
    IconModule,
    CommonModule,
    ReactiveFormsModule,
    ChartjsModule,
    ProgressSpinnerModule,
    PanelModule,
    BlockUIModule,
    PaginatorModule,
    BadgeModule,
    CheckboxModule,
    TableModule,
    KeyFilterModule,
    DuanSharedModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    DropdownModule,
    EditorModule,
    InputNumberModule,
    ImageModule,
    AutoCompleteModule,
    DynamicDialogModule,
  ],
  declarations: [
    PostCategoriesComponent,
    PostCategoryDetailComponent,
    PostComponent,
    PostActivityLogsComponent,
    PostDetailComponent,
    PostReturnReasonComponent,
    PostSeriesComponent,
    SeriesComponent,
    SeriesDetailComponent,
    SeriesPostsComponent,
  ],
})
export class ContentModule {}
