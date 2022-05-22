//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
module;

#include <vector>
#include <string>
#include <optional>
#include <memory>

#include <Windows.h>
#include <Msi.h>

#pragma comment (lib, "msi.lib")
export module ProductInfo;

using std::vector;
using std::wstring;
using std::optional;
using std::nullopt;
using std::unique_ptr;
using std::make_unique;


namespace product_info {

	/// <summary>
	/// MSI Product information
	/// </summary>
	export class msi_product {
        wstring product_code_{};

    public:
        explicit msi_product(wstring const& product_code) : product_code_{product_code} {}

        /// <summary>
        /// get version or nullopt
        /// </summary>
        /// <returns>version string or nullopt</returns>
        optional<wstring> get_version() const noexcept {
            auto version = get_version_from_product_info();
            if (!version.has_value()) {
                version = get_version_from_registry();
            }

            return version;
        }

    private:
        [[nodiscard]] optional<wstring> get_version_from_product_info() const noexcept {

            auto buffer = std::make_unique<wchar_t[]>(512);
            DWORD length{512};

            if (0u != MsiGetProductInfoW(product_code_.c_str(), L"INSTALLPROPERTY_VERSIONSTRING", buffer.get(), &length)) {
				return nullopt;
            }

			return {wstring(buffer.get(), length)};
        }
        [[nodiscard]] optional<wstring> get_version_from_registry() const noexcept {
            return nullopt;
        }

    };

	/// <summary>
	/// gets the product codes related to upgrade_code
	/// </summary>
	/// <param name="upgrade_code">the upgrade code to retrieve product codes for</param>
	/// <returns>std::vector<std::wstring> containing the product codes that match upgrade_code</returns>
    export [[nodiscard]] vector<msi_product> get_related_products(wstring upgrade_code) {
        return vector<msi_product>();
    }

} // namespace product_info