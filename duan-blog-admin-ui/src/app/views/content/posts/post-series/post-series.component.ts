import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  AddPostSeriesRequest,
  AdminApiPostApiClient,
  AdminApiSeriesApiClient,
  PostDto,
  SeriesInListDto,
} from '../../../../api/admin-api.service.generated';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { UtilityService } from '../../../../shared/services/utility-service';
import { AlertService } from '../../../../shared/services/alert.service';
import { MessageConstants } from '../../../../shared/constants/message-constant';

@Component({
  selector: 'app-post-series',
  templateUrl: './post-series.component.html',
  styleUrls: ['./post-series.component.scss'],
})
export class PostSeriesComponent implements OnInit, OnDestroy {
  //#region  variables
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public allSeries: any[] = [];
  public postSeries: any[];
  public selectedEntity: PostDto;

  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    seriesId: [{ type: 'required', message: 'Bạn phải chọn loạt bài' }],
    sortOrder: [{ type: 'required', message: 'Bạn phải nhập thứ tự' }],
  };
  //#endregion
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private utilService: UtilityService,
    private fb: FormBuilder,
    private postApiClient: AdminApiPostApiClient,
    private seriesApiClient: AdminApiSeriesApiClient,
    private alertService: AlertService
  ) {}
  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    //Init form
    this.buildForm();
    //Load data to form
    var series = this.seriesApiClient.getAllSeries();
    this.toggleBlockUI(true);
    forkJoin({
      series,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (repsonse: any) => {
          //Push categories to dropdown list
          var series = repsonse.series as SeriesInListDto[];
          series.forEach((element) => {
            this.allSeries.push({
              value: element.id,
              label: element.name,
            });
          });

          if (this.utilService.isEmpty(this.config.data?.id) == false) {
            this.loadSeries(this.config.data?.id);
          } else {
            this.toggleBlockUI(false);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  //#region  function
  saveChange() {
    this.toggleBlockUI(true);
    var body: AddPostSeriesRequest = new AddPostSeriesRequest({
      postId: this.config.data.id,
      seriesId: this.form.controls['seriesId'].value,
      sortOrder: this.form.controls['sortOrder'].value,
    });
    this.seriesApiClient
      .addPostSeries(body)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.alertService.showSuccess('Đã thêm bài viết thành công');
          this.loadSeries(this.config.data?.id);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
  //#endregion

  //#region  private

  loadSeries(id: string) {
    this.postApiClient
      .getSeriesBelong(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: SeriesInListDto[]) => {
          this.postSeries = response;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  removeSeries(id: string) {
    var body: AddPostSeriesRequest = new AddPostSeriesRequest({
      postId: this.config.data.id,
      seriesId: id,
    });
    this.seriesApiClient
      .deletePostSeries(body)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
          this.loadSeries(this.config.data?.id);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }
  buildForm() {
    this.form = this.fb.group({
      seriesId: new FormControl(null, Validators.required),
      sortOrder: new FormControl(0, Validators.required),
    });
  }
  //#endregion
}
