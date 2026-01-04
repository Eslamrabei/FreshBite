import { Component, inject, OnInit, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AdminService } from "../admin.service";
import { Router, RouterLink } from "@angular/router";
import { Brand, ProductType } from "../../shared/models/pagination";
import { environment } from "../../../environments/environment";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-admin-edit',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './admin-create.component.html',
  styleUrl: './admin-create.component.scss'
})

export class AdminCreateComponent implements OnInit {
  private fb = inject(FormBuilder);
  private adminService = inject(AdminService);
  private route = inject(Router);
  private toaster = inject(ToastrService);

  img = environment.imgUrl;
  productForm: FormGroup = new FormGroup({});
  brands = signal<Brand[]>([]);
  types = signal<ProductType[]>([]);
  isUploading = signal(false);

  ngOnInit() {
    this.initializeForm();
    this.loadBrandsAndTypes();
  }

  initializeForm() {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(1)]],
      description: ['', Validators.required],
      pictureUrl: [''],
      brandId: [null, Validators.required],
      typeId: [null, Validators.required]
    })
  }

  loadBrandsAndTypes() {
    this.adminService.getBrands().subscribe(b => this.brands.set(b));
    this.adminService.getTypes().subscribe(t => this.types.set(t));
  }

  onSubmit() {
    if (this.productForm.invalid) return;

    this.adminService.createProduct(this.productForm.value).subscribe({
      next: () => this.route.navigate(['/admin/product']),
      error: err => console.log(err)
    })
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.isUploading.set(true);
      this.adminService.uploadImage(file).subscribe({
        next: (response) => {
          this.productForm.patchValue({
            pictureUrl: response.filePath
          });

          this.isUploading.set(false);
          this.toaster.success('Image uploaded! Don\'t forget to click Create.');
        },
        error: (err) => {
          console.error(err);
          this.isUploading.set(false);
          this.toaster.error('Failed to upload image');
        }
      })
    }
  }


}
