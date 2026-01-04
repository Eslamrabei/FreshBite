import { Component, inject, OnInit, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AdminService } from "../admin.service";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Brand, ProductType } from "../../shared/models/pagination";
import { ToastrService } from "ngx-toastr";
import { environment } from "../../../environments/environment";


@Component({
  selector: 'app-admin-edit',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './admin-edit.component.html',
  styleUrl: './admin-edit.component.scss'
})

export class AdminEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private adminService = inject(AdminService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toaster = inject(ToastrService);


  productForm: FormGroup = new FormGroup({});
  brands = signal<Brand[]>([]);
  types = signal<ProductType[]>([]);
  productId: number = 0;
  isUploading = signal(false);
  img = environment.imgUrl;
  // update img
  imagePreview = signal('');
  selectedFile = signal<File | null>(null);



  ngOnInit(): void {
    this.initializeForm();
    this.loadBrandsAndTypes();
    this.loadProductData();
  }

  initializeForm() {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0.1)]],
      pictureUrl: [''],  // ??
      brandId: [null, Validators.required],
      typeId: [null, Validators.required]
    });
  }

  loadBrandsAndTypes() {
    this.adminService.getBrands().subscribe(b => this.brands.set(b));
    this.adminService.getTypes().subscribe(t => this.types.set(t));
  }

  loadProductData() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.productId = +id;
      this.adminService.getProductById(this.productId).subscribe({
        next: (product) => {
          this.productForm.patchValue(product);
          this.imagePreview.set(product.pictureUrl);
          console.log(this.imagePreview());
        }
      })
    }
  }

  onImageChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile.set(file);

      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview.set(e.target.result);
      }
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    if (this.productForm.invalid) return;

    const productData = { ...this.productForm.value, id: this.productId };

    this.adminService.updateProduct(this.productId, productData, this.selectedFile()).subscribe({
      next: () => this.router.navigate(['/admin/products']),
      error: err => console.log(err)
    })

  }




}
