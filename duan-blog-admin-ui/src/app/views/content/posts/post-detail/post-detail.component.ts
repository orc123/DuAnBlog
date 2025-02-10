import { Component, EventEmitter } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  AdminApiPostApiClient,
  AdminApiPostCategoryApiClient,
  PostCategoryDto,
  PostDto,
} from '../../../../api/admin-api.service.generated';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { UtilityService } from '../../../../shared/services/utility-service';
import { UploadService } from '../../../../shared/services/upload.service';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { environment } from '../../../../../environments/environment';
@Component({
  selector: 'app-post-detail',
  templateUrl: './post-detail.component.html',
  styleUrls: ['./post-detail.component.scss'],
})
export class PostDetailComponent {
  //#region  variables
  public blockedPanelDetail: boolean = false;
  private ngUnsubscribe = new Subject<void>();
  public form: FormGroup;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public postCategories: any = [];
  public contentType: any[] = [];
  public series: any[] = [];
  selectedEntity = {} as PostDto;
  public thumbnailImage;

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  // Validate
  public noSpecial: RegExp = /^[^<>*!_~]+$/;
  public validationMessages = {
    name: [
      { type: 'required', message: 'Bạn phải nhập tên' },
      { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
    ],
    slug: [{ type: 'required', message: 'Bạn phải URL duy nhất' }],
    description: [{ type: 'required', message: 'Bạn phải nhập mô tả ngắn' }],
  };
  //#endregion
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private utilService: UtilityService,
    private fb: FormBuilder,
    private postApiClient: AdminApiPostApiClient,
    private postCategoryApiClient: AdminApiPostCategoryApiClient,
    private uploadService: UploadService
  ) {}
  ngOnInit(): void {
    this.buildForm();
    var categories = this.postCategoryApiClient.getPostCategories();

    this.toggleBlockUI(true);
    forkJoin({ categories })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: any) => {
          var categories = response.categories as PostCategoryDto[];
          categories.forEach((element) => {
            this.postCategories.push({
              value: element.id,
              label: element.name,
            });
          });
          if (this.utilService.isEmpty(this.config.data?.id) == false) {
            this.loadFormDetails(this.config.data?.id);
          } else {
            this.toggleBlockUI(false);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadFormDetails(id: string) {
    this.postApiClient
      .getPostById(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  //#region  function

  saveChanges() {
    debugger;
    this.toggleBlockUI(true);
    if (this.utilService.isEmpty(this.config.data?.id)) {
      this.postApiClient
        .createPost(this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.ref.close(this.form.value);
            this.toggleBlockUI(false);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    } else {
      this.postApiClient
        .updatePost(this.config.data?.id, this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);

            this.ref.close(this.form.value);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    }
  }

  onFileChange(event) {
    if (event.target.files && event.target.files.length) {
      this.uploadService.uploadImage('posts', event.target.files).subscribe({
        next: (response: any) => {
          this.form.controls['thumbnail'].setValue(response.path);
          this.thumbnailImage = environment.API_URL + response.path;
        },
        error: (err: any) => {
          console.log(err);
        },
      });
    }
  }

  public generateSlug() {
    var slug = this.utilService.makeSeoTitle(this.form.get('name').value);
    this.form.controls['slug'].setValue(slug);
  }
  //#endregion

  //#region  private

  buildForm() {
    this.form = this.fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([
          Validators.required,
          Validators.maxLength(255),
          Validators.minLength(3),
        ])
      ),
      slug: new FormControl(
        this.selectedEntity.slug || null,
        Validators.required
      ),
      categoryId: new FormControl(
        this.selectedEntity.categoryId || null,
        Validators.required
      ),
      description: new FormControl(
        this.selectedEntity.description || null,
        Validators.required
      ),
      seoDescription: new FormControl(
        this.selectedEntity.seoDescription || null
      ),
      tags: new FormControl(this.selectedEntity.tags || null),
      content: new FormControl(this.selectedEntity.content || null),
      thumbnail: new FormControl(this.selectedEntity.thumbnail || null),
    });
    if (this.selectedEntity.thumbnail) {
      this.thumbnailImage = environment.API_URL + this.selectedEntity.thumbnail;
    }
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }
  //#endregion
}
