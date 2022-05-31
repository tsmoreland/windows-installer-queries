//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#ifndef GLOG_NO_ABBREVIATED_SEVERITIES
#define GLOG_NO_ABBREVIATED_SEVERITIES // NOLINT(clang-diagnostic-macro-redefined)
#endif

#include <glog/logging.h>
#include <iostream>

#include <Windows.h>

import Guid;
import ProductInfo;

using win32::guid;

int main(int const argc, char* argv[]) {
    if (char path[MAX_PATH + 1]{}; GetCurrentDirectoryA(MAX_PATH, path) != 0) {
        FLAGS_log_dir = path;
    }

    google::InitGoogleLogging(argv[0]);

    if (argc < 2) {
        std::wcout << L"Insufficient arguments" << std::endl;
        return -1;
    }

    guid const upgrade_code{argv[1]};

    for (auto const products = product_info::get_related_products(upgrade_code);
         auto const& product : products) {


        if (auto const version = product.get_version(); version.has_value()) {
            std::wcout << product.product_code() << L": " << version.value() << std::endl;
        } else {
            LOG(INFO) << "failed to get version";
        }
    }

    return 0;
}
