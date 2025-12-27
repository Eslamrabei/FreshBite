import { Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { AiSearchService } from '../../core/services/ai-search.service';
import { ChatMessage, ProductSearchResponse } from '../../shared/models/product-search-response';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe, NgClass, PercentPipe } from '@angular/common';

@Component({
  selector: 'app-ai-chat',
  standalone: true,
  imports: [FormsModule, CurrencyPipe, PercentPipe, NgClass],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent {
  private searchService = inject(AiSearchService);
  searchTerm = '';
  aiResults = signal<ProductSearchResponse[]>([]);
  hasSearched = signal(false);
  isLoading = signal(false);

  @ViewChild('chatContainer') private chatContainer!: ElementRef;
  scrollToBottom(): void {
    try {
      this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }



  messages = signal<ChatMessage[]>([
    { type: 'bot', text: '👋 Hi! I can find healthy food for you. Try "High protein dinner".' }
  ]);

  isOpen = signal(false);
  toggleChat() {
    this.isOpen.update(v => !v);
  }

  onSearch() {
    const rawText = this.searchTerm;
    if (!rawText) return;

    this.messages.update(msgs => [...msgs, { type: 'user', text: rawText }]);

    this.searchTerm = '';
    this.isLoading.set(true);

    const priceRegex = /(?:price|budget|under)\s*(\d+)/i;
    const match = rawText.match(priceRegex);
    let cleanQuery = rawText;
    let detectedPrice: number | undefined = undefined;

    if (match) {
      detectedPrice = parseInt(match[1]);
      cleanQuery = rawText.replace(match[0], '').trim().replace(/,\s*$/, '');
    }

    this.searchService.search(cleanQuery, detectedPrice).subscribe({
      next: (response: any) => {
        this.isLoading.set(false);

        console.log(response);

        if (response.aiAnswer) {
          this.messages.update(msgs => [...msgs, {
            type: 'bot',
            text: response.aiAnswer,
            products: response.products
          }]);

          setTimeout(() => this.scrollToBottom(), 100);

        } else {
          this.messages.update(msgs => [...msgs, {
            type: 'bot',
            text: '😓 My brain is offline (Connection Error). Please make sure Ollama is running!'
          }]);
          setTimeout(() => this.scrollToBottom(), 100);
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        console.log(err);
      }
    });
  }

  showAllSuggestions = signal(false);

  suggestions = [
    'Healthy Breakfast 🥞',
    'Under 50 EGP 💰',
    'Gym Protein 💪',
    'Cheat Meal 🍔',
    'Sweet Snacks 🍫',
    'Vegetarian 🥗',
    'Late Night Snack 🌙'
  ];
  toggleSuggestions() {
    this.showAllSuggestions.update(v => !v);
  }

  useSuggestion(text: string) {
    this.searchTerm = text;
    this.onSearch();
  }


}
